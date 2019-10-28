using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.TestFramework;
using Coderr.IntegrationTests.Core.Tools;
using Coderr.Server.Api.Client;
using Coderr.Server.Api.Core.Environments.Commands;
using Coderr.Server.Api.Core.Incidents.Queries;
using FluentAssertions;

namespace Coderr.IntegrationTests.Core.TestCases
{
    public class EnvironmentTests
    {
        private readonly ApplicationClient _applicationClient;
        private readonly ServerApiClient _apiClient;

        public EnvironmentTests(ApplicationClient applicationClient, ServerApiClient apiClient)
        {
            _applicationClient = applicationClient;
            _apiClient = apiClient;
        }

        [Test, RunOnlyThisOne]
        public async Task Clearing_environment_should_remove_all_incidents_in_it()
        {
            await _applicationClient.CreateIncident(x => { x.EnvironmentName = "Mock"; });

            await _apiClient.Reset(_applicationClient.ApplicationId, "Mock");

            var actual = await _apiClient.QueryAsync(new FindIncidents()
                {ApplicationIds = new[] {_applicationClient.ApplicationId}});
            actual.Items.Should().BeEmpty();
        }
    }
}
