using System;
using System.Collections.Generic;
using System.Text;

namespace fb_auto_like
{
    public class AppSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TwoFASeed { get; set; }
        public string[] IncludeUsers { get; set; }
        public string[] ExcludedUsers { get; set; }
    }
}
