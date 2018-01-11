using System;
using codeRR.Server.Web.Tests.Helpers.Extensions;
using codeRR.Server.Web.Tests.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class NavigationTests : LoggedInTest, IDisposable
    {
        public NavigationTests()
        {
            Login();
        }

        [Fact]
        public void Should_be_able_to_navigate_to_dashboard()
        {
            UITest(() =>
            {
                var sut = new HomePage(WebDriver);
                sut.NavigateToPage();
                sut.NavigationDashboard.Click();

                Assert.Equal("Overview", WebDriver.WaitUntilTitleEquals("Overview"));
            });
        }

        [Fact]
        public void Should_be_able_to_navigate_to_dashboard_overview()
        {
            UITest(() =>
            {
                var sut = new HomePage(WebDriver);
                sut.NavigateToPage();
                sut.NavigationDashboardOverview.Click();

                Assert.Equal("Overview", WebDriver.WaitUntilTitleEquals("Overview"));
            });
        }

        [Fact]
        public void Should_be_able_to_navigate_to_dashboard_incidents()
        {
            UITest(() =>
            {
                var sut = new HomePage(WebDriver);
                sut.NavigateToPage();
                sut.NavigationDashboardIncidents.Click();

                Assert.Equal("Incidents", WebDriver.WaitUntilTitleEquals("Incidents"));
            });
        }

        [Fact]
        public void Should_be_able_to_navigate_to_dashboard_feedback()
        {
            UITest(() =>
            {
                var sut = new HomePage(WebDriver);
                sut.NavigateToPage();
                sut.NavigationDashboardFeedback.Click();

                Assert.Equal("All feedback", WebDriver.WaitUntilTitleEquals("All feedback"));
            });
        }

        public void Dispose()
        {
            Logout();
        }
    }
}
