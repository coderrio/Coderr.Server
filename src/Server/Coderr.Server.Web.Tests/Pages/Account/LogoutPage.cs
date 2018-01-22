using OpenQA.Selenium;

namespace codeRR.Server.Web.Tests.Pages.Account
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
