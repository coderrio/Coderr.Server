using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.Tools;

namespace Coderr.IntegrationTests.Core
{
    public class ApplicationClient
    {
        private int _applicationId;
        private string _serverAddress;
        private string _dbName;
        private ReportingApiClient _apiClient;
        private Reporter _reporter;

        public ApplicationClient(string serverAddress, string dbName)
        {
            _serverAddress = serverAddress;
            _dbName = dbName;
        }

        public async Task Open()
        {
            _apiClient = await CreateApiClient();
            _reporter = CreateReporter(_applicationId);
        }

        private Reporter CreateReporter(int applicationId)
        {
            SqlTools.GetAppKey(_dbName, applicationId, out var appKey, out var appSecret);
            var reporter = new Reporter(new Uri(_serverAddress), appKey, appSecret);
            return reporter;
        }

        private async Task<ReportingApiClient> CreateApiClient()
        {
            SqlTools.GetApiKey(_dbName, out var apiKey, out var apiSecret);
            var apiClient = new ReportingApiClient(new Uri(_serverAddress), apiKey, apiSecret);
            _applicationId = await apiClient.EnsureApplication("ForTests");
            await apiClient.Reset(_applicationId, "IntegrationTests");
            return apiClient;
        }

        public async Task<IncidentWrapper> CreateIncident()
        {
            var report = _reporter.ReportUnique("LifeCycle");
            var incident = await _apiClient.GetIncident(_applicationId, report.Exception.Message);
            return new IncidentWrapper(this, report, incident);
        }
    }
}
