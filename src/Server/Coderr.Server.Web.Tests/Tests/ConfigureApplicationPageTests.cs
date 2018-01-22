using System;
using codeRR.Server.Web.Tests.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class ConfigureApplicationPageTests : LoggedInTest, IDisposable
    {
        public ConfigureApplicationPageTests()
        {
            Login();
        }

        [Fact]
        public void Should_not_be_able_to_create_application_without_name_specified()
        {
            UITest(() =>
            {
                var sut = new ConfigureApplicationPage(WebDriver)
                    .CreateApplication(string.Empty);

                //TODO: Verify error message
                sut.VerifyIsCurrentPage();
            });
        }

        [Fact]
        public void Should_be_able_to_create_application()
        {
            UITest(() =>
            {
                var applicationName = "TestApplication";

                var sut = new ConfigureApplicationPage(WebDriver)
                    .CreateApplication(applicationName);

                sut.VerifySuccessfullyCreatedApplication(applicationName);

                var homePage = new HomePage(WebDriver);
                homePage.NavigateToPage();
                homePage.VerifyIsCurrentPage();

                homePage.HasApplicationInNavigation(applicationName);
            });
        }

        public void Dispose()
        {
            Logout();
        }
    }
}
