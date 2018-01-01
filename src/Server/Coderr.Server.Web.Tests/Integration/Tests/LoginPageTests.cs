using codeRR.Server.Web.Tests.Integration.Fixtures;
using codeRR.Server.Web.Tests.Integration.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Integration.Tests
{
    [Collection("CommunityServerCollection")]
    [Trait("Category", "Integration")]
    public class LoginPageTests : CommunityServerTestBase
    {
        private readonly CommunityServerFixture _fixture;

        public LoginPageTests(CommunityServerFixture fixture) : base(fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Should_not_be_able_to_login_with_empty_username()
        {
            UITest(() =>
            {
                var sut = new LoginPage(_fixture.WebDriver)
                    .LoginWithNoUserNameSpecified();

                Assert.IsType<LoginPage>(sut);
                sut.VerifyIsCurrentPage();
            });
        }

        [Fact]
        public void Should_not_be_able_to_login_with_empty_password()
        {
            UITest(() =>
            {
                var sut = new LoginPage(_fixture.WebDriver)
                    .LoginWithNoPasswordSpecified();

                Assert.IsType<LoginPage>(sut);
                sut.VerifyIsCurrentPage();
            });
        }

        [Fact]
        public void Should_not_be_able_to_login_with_wrong_password()
        {
            UITest(() =>
            {
                var sut = new LoginPage(_fixture.WebDriver)
                    .LoginWithWrongPasswordSpecified();

                Assert.IsType<LoginPage>(sut);
                sut.VerifyIsCurrentPage();
            });
        }

        [Fact]
        public void Should_be_able_to_login_with_valid_credentials()
        {
            UITest(() =>
            {
                var sut = new LoginPage(_fixture.WebDriver)
                    .LoginWithValidCredentials();

                Assert.IsType<HomePage>(sut);
                sut.VerifyIsCurrentPage();

                Logout();
            });
        }
    }
}
