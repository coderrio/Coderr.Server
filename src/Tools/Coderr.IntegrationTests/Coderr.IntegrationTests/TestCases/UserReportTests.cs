using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.IntegrationTests.Core.Entities;
using Coderr.IntegrationTests.Core.Tools;
using Coderr.Server.Api.Client;
using Coderr.Server.Api.Core.Reports.Queries;
using Coderr.Server.Api.Web.Feedback.Queries;
using Coderr.Tests.Attributes;
using FluentAssertions;

namespace Coderr.IntegrationTests.Core.TestCases
{
    public class UserReportTests
    {
        private readonly ApplicationClient _applicationClient;
        private readonly ServerApiClient _apiClient;

        public UserReportTests(ApplicationClient applicationClient, ServerApiClient apiClient)
        {
            _applicationClient = applicationClient;
            _apiClient = apiClient;
        }

        [Test]
        public async Task Should_be_able_to_attach_feedback_to_existing_report()
        {
            var incident = await _applicationClient.CreateIncident(new {helloWorld = true});
            await incident.LeaveFeedback("Hello world");
        }

        [Test]
        public async Task Should_be_able_to_attach_feedback_when_reporting()
        {
            var incident = await _applicationClient.CreateIncident(new[]
            {
                new ContextCollectionDTO("UserSuppliedInformation",
                    new Dictionary<string, string>
                    {
                        {"EmailAddress", "jonas@somewhere.com"},
                        {"Description", "Hello world"}
                    })
            });

            var actual = await GetInformation(incident);

            actual.Property("EmailAddress").Should().Be("jonas@somewhere.com");
            actual.Property("Description").Should().Be("Hello world");
        }

        [Test]
        public async Task Should_be_able_to_attach_only_email_address_when_reporting()
        {
            var incident = await _applicationClient.CreateIncident(new[]
            {
                new ContextCollectionDTO("UserSuppliedInformation",
                    new Dictionary<string, string>
                    {
                        {"EmailAddress", "jonas1@somewhere.com"}
                    })
            });

            var actual = await GetInformation(incident);

            actual.Property("EmailAddress").Should().Be("jonas1@somewhere.com");
        }

        [Test]
        public async Task Should_be_able_to_attach_only_feedback_when_reporting()
        {
            var incident = await _applicationClient.CreateIncident(new[]
            {
                new ContextCollectionDTO("UserSuppliedInformation",
                    new Dictionary<string, string>
                    {
                        {"Description", "Hello world"}
                    })
            });

            var actual = await GetInformation(incident);


            actual.Property("Description").Should().Be("Hello world");
        }

        [Test]
        public async Task Should_be_able_to_retrieve_feedback()
        {
            var incident = await _applicationClient.CreateIncident(new[]
            {
                new ContextCollectionDTO("UserSuppliedInformation",
                    new Dictionary<string, string>
                    {
                        {"Description", "Hello world"}
                    })
            });
            await GetInformation(incident);

            var query = new GetFeedbackForApplicationPage(_applicationClient.ApplicationId);
            var result = await _apiClient.QueryAsync(query);

            result.Items[0].Message.Should().Be("Hello world");
        }


        private async Task<GetReportResultContextCollection> GetInformation(IncidentWrapper incident)
        {
            var report =
                await incident.GetReport(x => x.ContextCollections.Any(y => y.Name == "UserSuppliedInformation"));
            return report.ContextCollections.FirstOrDefault(x => x.Name == "UserSuppliedInformation");
        }
    }
}