using codeRR.Server.Web.Tests.Integration.Fixtures;
using codeRR.Server.Web.Tests.Integration.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Integration.Tests
{
    public class CommunityServerTestBase : TestBase
    {
        protected CommunityServerFixture Fixture { get; }

        public CommunityServerTestBase(CommunityServerFixture fixture) : base(fixture.WebDriver)
        {
            Fixture = fixture;
        }

        public HomePage Login()
        {
            var page = new LoginPage(Fixture.WebDriver)
                .LoginWithValidCredentials();

            Assert.IsType<HomePage>(page);

            page.VerifyIsCurrentPage();

            return page;
        }

        public LoginPage Logout()
        {
            var page = new LogoutPage(Fixture.WebDriver)
                .Logout();

            page.VerifyIsCurrentPage();

            return page;
        }
    }
}
