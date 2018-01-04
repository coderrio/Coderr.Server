using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Pages
{
    public class ConfigureApplicationPage : BasePage
    {
        public ConfigureApplicationPage(IWebDriver webDriver) : base(webDriver, "configure/application", "Create a new application - codeRR")
        {
        }

        [FindsBy(How = How.ClassName, Using = "btn-primary")]
        public IWebElement CreateButton { get; set; }

        [FindsBy(How = How.Name, Using = "Name")]
        public IWebElement ApplicationNameField { get; set; }

        public ConfigureApplicationPage CreateApplication(string name)
        {
            NavigateToPage();

            ApplicationNameField.Clear();
            ApplicationNameField.SendKeys(name);

            CreateButton.Click();

            return this;
        }

        public void VerifyIsCurrentPage()
        {
            Wait.Until(ExpectedConditions.TitleIs(Title));
        }

        public void VerifySuccessfullyCreatedApplication(string name)
        {
            Wait.Until(ExpectedConditions.TitleIs(name));
        }
    }
}
