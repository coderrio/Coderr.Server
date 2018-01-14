using codeRR.Server.Web.Tests.Pages;
using codeRR.Server.Web.Tests.Pages.Account;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    public class LoggedInTest : WebTest
    {
        public HomePage Login()
        {
            var page = new LoginPage(WebDriver)
                .LoginWithValidCredentials();

            Assert.IsType<HomePage>(page);

            page.VerifyIsCurrentPage();

            return page;
        }

        public LoginPage Logout()
        {
            var page = new LogoutPage(WebDriver)
                .Logout();

            page.VerifyIsCurrentPage();

            return page;
        }
    }
}
