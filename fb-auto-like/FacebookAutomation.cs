using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using OtpNet;
using System;
using System.Linq;
using System.Threading;

namespace fb_auto_like
{
    public class FacebookAutomation
    {
        private readonly IWebDriver _webDriver;
        private readonly AppSettings _appSettings;

        public FacebookAutomation(IWebDriver webDriver, AppSettings appSettings)
        {
            _webDriver = webDriver;
            _appSettings = appSettings;
        }

        public void Login()
        {
            // Navigate to Facebook
            _webDriver.Url = "https://www.facebook.com/";

            // Find the username field (Facebook calls it "email") and enter value
            var input = _webDriver.FindElement(By.Id("email"));
            input.SendKeys(_appSettings.UserName);

            // Find the password field and enter value
            input = _webDriver.FindElement(By.Id("pass"));
            input.SendKeys(_appSettings.Password);

            // Click on the login button
            ClickAndWaitForPageToLoad(_webDriver, By.Name("login"));

            if (!string.IsNullOrWhiteSpace(_appSettings.TwoFASeed))
            {
                // A 2FA seed code was passed, let's generate the 2FA code
                var otpKeyBytes = Base32Encoding.ToBytes(_appSettings.TwoFASeed);
                var totp = new Totp(otpKeyBytes);
                var twoFactorCode = totp.ComputeTotp();

                // Enter the code into the UI
                input = _webDriver.FindElement(By.Id("approvals_code"));
                input.SendKeys(twoFactorCode);
            }

            // At this point, Facebook will launch a post-login "wizard" that will 
            // keep asking unknown amount of questions (it thinks it's the first time 
            // you logged in using this computer). We'll just click on the "continue" 
            // button until they give up and redirect us to our "wall".
            try
            {
                while (_webDriver.FindElement(By.Id("checkpointSubmitButton")) != null)
                {
                    // Clicking "continue" until we're done
                    ClickAndWaitForPageToLoad(_webDriver, By.Id("checkpointSubmitButton"));
                }
            }
            catch
            {
                // We will try to click on the next button until it's not there or we fail.
                // Facebook is unexpected as to what will happen, but this approach seems 
                // to be pretty reliable
            }

            for (int i = 0; i < 30; i++)
            {
                _webDriver.ExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight)");
                Thread.Sleep(3000);
            }

            var elements = _webDriver.FindElements(By.XPath("//div[@data-pagelet]"));

            foreach (var item in elements)
            {
                var attr = item.GetAttribute("data-pagelet");
                if (!attr.StartsWith("FeedUnit"))
                {
                    continue;
                }

                var text = item.Text;
                if (_appSettings.IncludeUsers != null)
                {
                    foreach (var includeUser in _appSettings.IncludeUsers)
                    {
                        if (!text.Contains(includeUser))
                        {
                            continue;
                        }
                    }
                }

                if (_appSettings.ExcludedUsers != null)
                {
                    foreach (var excludeUser in _appSettings.ExcludedUsers)
                    {
                        if (text.Contains(excludeUser))
                        {
                            continue;
                        }
                    }
                }

                if (text.Contains("updated his profile picture") || 
                    text.Contains("updated her profile picture") ||
                    text.Contains("updated his cover photo") ||
                    text.Contains("updated her cover photo"))
                {
                    var likeBtn = item.FindElement(By.XPath(".//div[@aria-label=\"Like\"]"));
                    var child = likeBtn.FindElement(By.XPath(".//div"));
                    if (string.IsNullOrEmpty(child.GetAttribute("style")))
                    {
                        likeBtn.Click();
                    }
                    else
                    {
                        // Already liked
                    }
                }

            }

            Console.WriteLine("Finished automation");
        }

        private void ClickAndWaitForPageToLoad(IWebDriver driver,
            By elementLocator, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                var elements = driver.FindElements(elementLocator);
                if (elements.Count == 0)
                {
                    throw new NoSuchElementException(
                        "No elements " + elementLocator + " ClickAndWaitForPageToLoad");
                }
                var element = elements.FirstOrDefault(e => e.Displayed);
                element.Click();
                wait.Until(ExpectedConditions.StalenessOf(element));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with locator: '" + elementLocator + "' was not found.");
                throw;
            }
        }
    }
}
