using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.IntegrationTests.Core.TestFramework;
using Coderr.IntegrationTests.Core.Tools;
using Coderr.Server.Api.Client;
using Coderr.Server.Api.Core.Environments.Queries;
using Coderr.Server.Api.Core.Incidents.Commands;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Api.Core.Reports.Queries;
using Coderr.Server.Api.Web.Feedback.Queries;

namespace Coderr.IntegrationTests.Core.Entities
{
    public class IncidentWrapper
    {
        private readonly ServerApiClient _apiClient;
        private ErrorReportDTO _report;
        private GetIncidentResult _incident;
        private Reporter _reporter;
        private readonly int _applicationId;

        public IncidentWrapper(ServerApiClient apiClient, Reporter reporter, int applicationId)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _applicationId = applicationId;
        }

        public GetIncidentResult DTO => _incident;
        public bool IsClosed => _incident.IsSolved;
        public string Environments => _incident.Facts.FirstOrDefault(x => x.Title == "Environments")?.Value ?? "";

        public async Task Create(Action<ErrorReportDTO> visitor = null)
        {
            _report = _reporter.ReportUnique(Guid.NewGuid().ToString("N"), visitor);
            _incident = await _apiClient.GetIncident(_applicationId, _report.Exception.Message);
        }

        public async Task Create(object contextData, Action<ErrorReportDTO> visitor = null)
        {
            _report = _reporter.ReportUnique(Guid.NewGuid().ToString("N"), contextData, visitor);
            _incident = await _apiClient.GetIncident(_applicationId, _report.Exception.Message);
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
                if (result.Solution != description)
                    return false;

                _incident = result;
                return true;
            }

            if (!await ActionExtensions.Retry(Func))
                throw new TestFailedException($"Incident {_incident.Id} was not closed.");
        }

        public async Task ReOpen(string version)
        {
            _reporter.ReportCopy(_report, Guid.NewGuid().ToString(), x =>
            {
                var col = x.ContextCollections.GetCoderrCollection();
                col.Properties["AppAssemblyVersion"] = version;
            });

            async Task<bool> Func()
            {
                var query = new GetIncident(_incident.Id);
                var result = await _apiClient.QueryAsync(query);
                if (!result.IsReOpened)
                    return false;

                _incident = result;
                return true;
            }

            if (!await ActionExtensions.Retry(Func))
                throw new TestFailedException($"Incident {_incident.Id} was not reopened.");
        }

        public async Task Report(Action<ErrorReportDTO> visitor = null)
        {
            var report = _reporter.ReportCopy(_report, Guid.NewGuid().ToString("N"), visitor);
            await _apiClient.GetReport(_incident.Id, report.Exception.Message);
        }

        public async Task Update(Func<GetIncidentResult, bool> filter = null)
        {
            if (filter == null)
                filter = x => x.UpdatedAtUtc != _incident.UpdatedAtUtc;

            async Task<bool> Func()
            {
                var query = new GetIncident(_incident.Id);
                var result = await _apiClient.QueryAsync(query);
                if (!filter(result))
                    return false;

                _incident = result;
                return true;
            }

            if (!await ActionExtensions.Retry(Func))
                throw new TestFailedException($"Incident {_incident.Id} was not updated.");
        }

        public async Task UpdateByLastReceivedReport(Func<GetIncidentResult, bool> filter = null)
        {
            await Update(x => x.LastReportReceivedAtUtc > _incident.LastReportReceivedAtUtc);
        }

        public async Task<GetReportResult> GetReport()
        {
            return await _apiClient.GetReport(_incident.Id, _report.Exception.Message);
        }

        public async Task LeaveFeedback(string message, string email = null)
        {
            _reporter.LeaveFeedback( _report.ReportId, message, email);

            async Task<bool> Func()
            {
                var query = new GetIncidentFeedback(_incident.Id);
                var result = await _apiClient.QueryAsync(query);
                var item = result.Items.FirstOrDefault(x => x.Message == message);
                return item != null;
            }

            if (!await ActionExtensions.Retry(Func))
                throw new TestFailedException($"Incident {_incident.Id} did not get the specified feedback.");
        }
    }
}
