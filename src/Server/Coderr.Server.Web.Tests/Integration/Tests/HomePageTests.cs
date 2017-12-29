using codeRR.Server.Web.Tests.Integration.Fixtures;
using codeRR.Server.Web.Tests.Integration.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Integration.Tests
{
    [Collection("CommunityServerCollection")]
    [Trait("Category", "Integration")]
    public class HomePageTests : CommunityServerTestBase
    {
        private readonly CommunityServerFixture _fixture;

        public HomePageTests(CommunityServerFixture fixture) : base(fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Should_be_able_to_navigate_to_application()
        {
            UITest(() =>
            {
                Login();

                var sut = new HomePage(_fixture.WebDriver);
                sut.NavigateToPage();

                sut.FirstApplication.Click();

                sut.VerifyNavigatedToFirstApplication();

                Logout();
            });
        }
    }
}
