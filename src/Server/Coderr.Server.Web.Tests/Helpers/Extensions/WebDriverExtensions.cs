using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Helpers.Extensions
{
    public static class WebDriverExtensions
    {
        public static bool WaitUntilElementIsPresent(this IWebDriver driver, By by, int timeout = 5)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return wait.Until(d => d.ElementIsPresent(by));
        }

        public static bool WaitUntilElementIsPresent(this IWebDriver driver, IWebElement element, int timeout = 5)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return wait.Until(d => d.ElementIsPresent(element));
        }

        public static string WaitUntilTitleEquals(this IWebDriver driver, string title, int timeout = 5)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                wait.Until(ExpectedConditions.TitleIs(title));
            }
            catch
            {
                // ignored
            }

            return driver.Title;
        }

        private static bool ElementIsPresent(this IWebDriver driver, By by)
        {
            var present = false;
            try
            {
                present = driver.FindElement(by).Displayed;
            }
            catch (NoSuchElementException)
            {
            }
            return present;
        }

        private static bool ElementIsPresent(this IWebDriver driver, IWebElement element)
        {
            var present = false;
            try
            {
                present = element.Displayed;
            }
            catch (NoSuchElementException)
            {
            }
            return present;
        }
    }
}
