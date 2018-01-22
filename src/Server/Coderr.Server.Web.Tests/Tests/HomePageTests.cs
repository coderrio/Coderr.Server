using System;
using codeRR.Server.Web.Tests.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class HomePageTests : LoggedInTest, IDisposable
    {
        public HomePageTests()
        {
            Login();
        }

        [Fact]
        public void Should_be_able_to_navigate_to_myfirstapp_application()
        {
            UITest(() =>
            {
                var sut = new HomePage(WebDriver);
                sut.NavigateToPage();

                sut.NavigationMyTestApp.Click();

                sut.VerifyNavigatedToMyTestApp();
            });
        }

        public void Dispose()
        {
            Logout();
        }
    }
}
