using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages
{
    public class ApplicationPage : BasePage
    {
        public ApplicationPage(IWebDriver webDriver, int id) : base(webDriver, "#/application/{id}", "")
        {
            Url = Url.Replace("{id}", id.ToString());
        }

        [FindsBy(How = How.Id, Using = "pageTitle")]
        public IWebElement PageTitle { get; set; }

        public void VerifyIsCurrentPage()
        {
            Wait.Until(ExpectedConditions.TitleIs(Title));
        }

        public void VerifyIncidentReported()
        {
            var by = By.PartialLinkText("Value cannot be null");
            //var element = WebDriver.FindElement(by);
            Wait.Until(ExpectedConditions.ElementExists(by));
        }
    }
}
