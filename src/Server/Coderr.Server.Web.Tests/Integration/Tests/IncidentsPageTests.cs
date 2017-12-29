using System;
using System.Threading;
using codeRR.Client;
using codeRR.Server.Web.Tests.Integration.Fixtures;
using codeRR.Server.Web.Tests.Integration.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Integration.Tests
{
    [Collection("CommunityServerCollection")]
    [Trait("Category", "Integration")]
    public class IncidentsPageTests : CommunityServerTestBase
    {
        private readonly CommunityServerFixture _fixture;

        public IncidentsPageTests(CommunityServerFixture fixture) : base(fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Should_be_able_to_report_error_with_client_lib_and_error_shows_up_in_incidents()
        {
            UITest(() =>
            {
                Login();

                var url = new Uri(_fixture.BaseUrl);
                Err.Configuration.Credentials(url, _fixture.Application.AppKey, _fixture.Application.SharedSecret);

                Err.Report(new ArgumentNullException("id"), new { SampleData = "Context example" });

                // Give the server some time to process the incident
                Thread.Sleep(3000);

                var sut = new IncidentsPage(_fixture.WebDriver, 1);
                sut.NavigateToPage();

                sut.VerifyIncidentReported();

                Logout();
            });
        }
    }
}
