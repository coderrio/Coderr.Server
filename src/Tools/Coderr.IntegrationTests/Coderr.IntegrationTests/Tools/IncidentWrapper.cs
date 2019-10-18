using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Server.Api.Client;
using Coderr.Server.Api.Core.Incidents.Commands;
using Coderr.Server.Api.Core.Incidents.Queries;

namespace Coderr.IntegrationTests.Core.Tools
{
    public class IncidentWrapper
    {
        private readonly ServerApiClient _apiClient;
        private readonly ErrorReportDTO _report;
        private GetIncidentResult _incident;

        public IncidentWrapper(ServerApiClient apiClient, ErrorReportDTO report, GetIncidentResult incident)
        {
            _apiClient = apiClient;
            _report = report;
            _incident = incident;
        }

        public async Task Assign(int userId)
        {
            await _apiClient.SendAsync(new AssignIncident(_incident.Id, userId, userId));

            async Task<bool> Func()
            {
                var query = new GetIncident(_incident.Id);
                var result = await _apiClient.QueryAsync(query);
                if (result.AssignedToId != userId) return false;

                _incident = result;
                return true;
            }

            if (!await ActionExtensions.Retry(Func))
                throw new TestFailedException($"Incident {_incident.Id} was not assigned to {userId}.");
        }

        public async Task Close(int closedBy, string description, string version)
        {
            var cmd = new CloseIncident(description, _incident.Id)
            {
                UserId = closedBy,
                ApplicationVersion = version
            };
            await _apiClient.SendAsync(cmd);

            async Task<bool> Func()
            {
                var query = new GetIncident(_incident.Id);
                var result = await _apiClient.QueryAsync(query);
                if (result.Solution != description) return false;

                _incident = result;
                return true;
            }

            if (!await ActionExtensions.Retry(Func))
                throw new TestFailedException($"Incident {_incident.Id} was not closed.");
        }

        public async Task Update()
        {
            async Task<bool> Func()
            {
                var query = new GetIncident(_incident.Id);
                var result = await _apiClient.QueryAsync(query);
                if (result.UpdatedAtUtc != _incident.UpdatedAtUtc)
                    return false;

                _incident = result;
                return true;
            }

            if (!await ActionExtensions.Retry(Func))
                throw new TestFailedException($"Incident {_incident.Id} was not updated.");
        }

    }
}
