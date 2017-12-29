using codeRR.Server.Web.Tests.Integration.Fixtures;
using codeRR.Server.Web.Tests.Integration.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Integration.Tests
{
    [Collection("CommunityServerCollection")]
    [Trait("Category", "Integration")]
    public class ConfigureApplicationPageTests : CommunityServerTestBase
    {
        private readonly CommunityServerFixture _fixture;

        public ConfigureApplicationPageTests(CommunityServerFixture fixture) : base(fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Should_not_be_able_to_create_application_without_name_specified()
        {
            UITest(() =>
            {
                Login();

                var sut = new ConfigureApplicationPage(_fixture.WebDriver)
                    .CreateApplication(string.Empty);

                sut.VerifyIsCurrentPage();

                Logout();
            });
        }

        [Fact]
        public void Should_be_able_to_create_application()
        {
            UITest(() =>
            {
                Login();

                var applicationName = "TestApplication";

                var sut = new ConfigureApplicationPage(_fixture.WebDriver)
                    .CreateApplication(applicationName);

                sut.VerifySuccessfullyCreatedApplication(applicationName);

                var homePage = new HomePage(_fixture.WebDriver);
                homePage.NavigateToPage();
                homePage.VerifyIsCurrentPage();

                homePage.HasApplicationInNavigation(applicationName);

                Logout();
            });
        }
    }
}
