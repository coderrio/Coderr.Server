using codeRR.Server.Web.Tests.Pages;
using codeRR.Server.Web.Tests.Pages.Account;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    public class LoggedInTest : WebTest
    {
        public IPage Login()
        {
            return Login(TestData.TestUser.Username, TestData.TestUser.Password);
        }

        public IPage Login(string userName, string password)
        {
            var page = new LoginPage(WebDriver)
                .LoginWithValidCredentials(userName, password);

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
