using System;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.IntegrationTests.Core.Entities;
using Coderr.IntegrationTests.Core.Tools;
using Coderr.Server.Api.Client;

namespace Coderr.IntegrationTests.Core
{
    public class ApplicationClient
    {
        private ServerApiClient _apiClient;
        private int _applicationId;
        private readonly string _dbName;
        private Reporter _reporter;
        private readonly string _serverAddress;

        public ApplicationClient(string serverAddress, string dbName)
        {
            _serverAddress = serverAddress;
            _dbName = dbName;
        }

        public async Task<IncidentWrapper> CreateIncident(Action<ErrorReportDTO> callback = null)
        {
            var wrapper = new IncidentWrapper(_apiClient, _reporter, _applicationId);
            await wrapper.Create(callback);
            return wrapper;
        }

        public async Task<IncidentWrapper> CreateIncident(object contextData, Action<ErrorReportDTO> callback = null)
        {
            var wrapper = new IncidentWrapper(_apiClient, _reporter, _applicationId);
            await wrapper.Create(contextData, callback);
            return wrapper;
        }


        public async Task Open()
        {
            _apiClient = await CreateApiClient();
            _reporter = CreateReporter(_applicationId);
        }

        private async Task<ServerApiClient> CreateApiClient()
        {
            SqlTools.GetApiKey(_dbName, out var apiKey, out var apiSecret);
            var apiClient = new ServerApiClient();
            apiClient.Open(new Uri(_serverAddress), apiKey, apiSecret);
            _applicationId = await apiClient.EnsureApplication("ForTests");
            await apiClient.Reset(_applicationId, "IntegrationTests");
            return apiClient;
        }

        private Reporter CreateReporter(int applicationId)
        {
            SqlTools.GetAppKey(_dbName, applicationId, out var appKey, out var appSecret);
            var reporter = new Reporter(new Uri(_serverAddress), appKey, appSecret);
            return reporter;
        }
    }
}