using codeRR.Server.Web.Tests.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class ConfigureApplicationPageTests : LoggedInTest
    {
        [Fact]
        public void Should_not_be_able_to_create_application_without_name_specified()
        {
            UITest(() =>
            {
                Login();

                var sut = new ConfigureApplicationPage(WebDriver)
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

                var sut = new ConfigureApplicationPage(WebDriver)
                    .CreateApplication(applicationName);

                sut.VerifySuccessfullyCreatedApplication(applicationName);

                var homePage = new HomePage(WebDriver);
                homePage.NavigateToPage();
                homePage.VerifyIsCurrentPage();

                homePage.HasApplicationInNavigation(applicationName);

                Logout();
            });
        }
    }
}
