using System;
using System.Text.RegularExpressions;
using codeRR.Server.Web.Tests.Pages.Account;
using OpenQA.Selenium;

namespace codeRR.Server.Web.Tests.Pages
{
    public class PageHelper
    {
        public static IPage ResolvePage(IWebDriver webDriver)
        {
            var match = Regex.Match(webDriver.Url, @"/#/$", RegexOptions.IgnoreCase);
            if (match.Success)
                return new HomePage(webDriver);
            match = Regex.Match(webDriver.Url, @"/Account/Login", RegexOptions.IgnoreCase);
            if (match.Success)
                return new LoginPage(webDriver);
            match = Regex.Match(webDriver.Url, @"/#/application/(\d+)", RegexOptions.IgnoreCase);
            if(match.Success)
                return new ApplicationPage(webDriver, Convert.ToInt16(match.Groups[1].Value));

            throw new ArgumentOutOfRangeException($"Url: {webDriver.Url}, Title: {webDriver.Title}");
        }
    }
}
