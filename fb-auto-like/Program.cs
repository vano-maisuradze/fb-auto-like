using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace fb_auto_like
{
    class Program
    {
        static void Main(string[] args)
        {
            var settingsJson = File.ReadAllText("appsettings.json");
            var settings = JsonConvert.DeserializeObject<AppSettings>(settingsJson);

            var webDriver = LaunchBrowser();
            try
            {
                var facebookAutomation = new FacebookAutomation(webDriver, settings);
                facebookAutomation.Login();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while executing automation");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                webDriver.Quit();
            }
        }

        static IWebDriver LaunchBrowser()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");

            var driver = new ChromeDriver(Environment.CurrentDirectory, options);
            return driver;
        }
    }
}
