using System;
using System.Threading;
using codeRR.Client;
using codeRR.Server.Web.Tests.Pages;
using Xunit;

namespace codeRR.Server.Web.Tests.Tests
{
    [Trait("Category", "Integration")]
    public class IncidentsPageTests : LoggedInTest, IDisposable
    {
        public IncidentsPageTests()
        {
            Login();
        }

        [Fact]
        public void Should_be_able_to_report_error_with_client_lib_and_error_shows_up_in_incidents()
        {
            UITest(() =>
            {
                var url = new Uri(ServerUrl);
                Err.Configuration.Credentials(url, TestData.Application.AppKey, TestData.Application.SharedSecret);
                Err.Report(new ArgumentNullException("id"), new { SampleData = "Context example" });

                // Give the server some time to process the incident
                Thread.Sleep(3000);

                var sut = new IncidentsPage(WebDriver, 1);
                sut.NavigateToPage();

                sut.VerifyIncidentReported();
            });
        }

        public void Dispose()
        {
            Logout();
        }
    }
}
