using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;

namespace codeRR.Server.Web.Tests.Helpers.Selenium
{
    public static class DriverFactory
    {
        public static IWebDriver Create(BrowserType browserType)
        {
            IWebDriver driver;

            switch (browserType)
            {
                case BrowserType.Chrome:
                    driver = new ChromeDriver();
                    break;

                case BrowserType.InternetExplorer:
                    driver = new InternetExplorerDriver();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
