using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Integration.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver webDriver) : base(webDriver, "", "MyTestApp")
        {
        }

        [FindsBy(How = How.XPath, Using = "//a/span[.=' MyTestApp ']")]
        public IWebElement FirstApplication { get; set; }

        [FindsBy(How = How.Id, Using = "pageTitle")]
        public IWebElement PageTitle { get; set; }

        public bool HasApplicationInNavigation(string applicationName)
        {
            var by = By.XPath($"//a/span[.=' {applicationName} ']");
            var element = WebDriver.FindElement(by);

            return true;
        }

        public void VerifyIsCurrentPage()
        {
            Wait.Until(ExpectedConditions.TitleIs("Overview"));
        }

        public void VerifyNavigatedToFirstApplication()
        {
            Wait.Until(ExpectedConditions.TextToBePresentInElement(PageTitle, Title));
        }
    }
}
