using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages
{
    public class BasePage
    {
        protected IWebDriver WebDriver;
        protected WebDriverWait Wait;
        protected string BaseUrl { get; }
        protected string Url { get; set; }
        protected string Title { get; }

        public BasePage(IWebDriver webDriver, string url, string title)
        {
            WebDriver = webDriver;
            Title = title;

            Wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));

            BaseUrl = "http://localhost:50473/coderr/";

            Url = new Uri(new Uri(BaseUrl), url).ToString();

            PageFactory.InitElements(WebDriver, this);
        }

        public void NavigateToPage(bool wait = true)
        {
            WebDriver.Navigate().GoToUrl(Url);

            if(wait)
                Wait.Until(ExpectedConditions.UrlContains(Url));
        }

        public void DeleteCookies()
        {
            WebDriver.Manage().Cookies.DeleteAllCookies();
        }

        public void Close()
        {
            WebDriver.Close();
            WebDriver.Dispose();
        }
    }
}
