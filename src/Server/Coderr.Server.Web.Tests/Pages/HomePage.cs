using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver webDriver) : base(webDriver, "", "Overview")
        {
        }

        [FindsBy(How = How.XPath, Using = "//a/span[.=' MyTestApp ']")]
        public IWebElement NavigationMyTestApp { get; set; }

        [FindsBy(How = How.Id, Using = "pageTitle")]
        public IWebElement PageTitle { get; set; }

        [FindsBy(How = How.XPath, Using = "//a/span[.=' Dashboard ']")]
        public IWebElement NavigationDashboard { get; set; }

        [FindsBy(How = How.XPath, Using = "//a[.='Overview']")]
        public IWebElement NavigationDashboardOverview { get; set; }

        [FindsBy(How = How.XPath, Using = "//a[.='Incidents']")]
        public IWebElement NavigationDashboardIncidents { get; set; }

        [FindsBy(How = How.XPath, Using = "//a[.='Feedback']")]
        public IWebElement NavigationDashboardFeedback { get; set; }

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

        public void VerifyNavigatedToMyTestApp()
        {
            Wait.Until(ExpectedConditions.TextToBePresentInElement(PageTitle, "MyTestApp"));
        }
    }
}
