using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace codeRR.Server.Web.Tests.Integration.Pages
{
    public class LogoutPage : BasePage
    {
        public LogoutPage(IWebDriver webDriver) : base(webDriver, "Account/Logout", "")
        {
        }

        public LoginPage Logout()
        {
            NavigateToPage(false);

            return new LoginPage(WebDriver);
        }
    }
}
