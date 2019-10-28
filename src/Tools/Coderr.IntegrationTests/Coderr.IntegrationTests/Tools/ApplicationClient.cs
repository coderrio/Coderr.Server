using System;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.IntegrationTests.Core.Entities;
using Coderr.Server.Api.Client;

namespace Coderr.IntegrationTests.Core.Tools
{
    public class ApplicationClient
    {
        private readonly string _dbName;
        private readonly string _serverAddress;
        private ServerApiClient _apiClient;
        private Reporter _reporter;

        public ApplicationClient(string serverAddress, string dbName)
        {
            _serverAddress = serverAddress;
            _dbName = dbName;
        }

        public int ApplicationId { get; private set; }

        public async Task<IncidentWrapper> CreateIncident(Action<ErrorReportDTO> callback = null)
        {
            var wrapper = new IncidentWrapper(_apiClient, _reporter, ApplicationId);
            await wrapper.Create(callback);
            return wrapper;
        }

        public async Task<IncidentWrapper> CreateIncidentWithoutSignature(Action<ErrorReportDTO> callback = null)
        {
            var wrapper = new IncidentWrapper(_apiClient, _reporter, ApplicationId);
            await wrapper.CreateWithoutSignature(callback);
            return wrapper;
        }

        public async Task<IncidentWrapper> CreateIncident(object contextData, Action<ErrorReportDTO> callback = null)
        {
            var wrapper = new IncidentWrapper(_apiClient, _reporter, ApplicationId);
            await wrapper.Create(contextData, callback);
            return wrapper;
        }


        public async Task<ServerApiClient> Open()
        {
            _apiClient = await CreateApiClient();
            _reporter = CreateReporter(ApplicationId);
            return _apiClient;
        }

        private async Task<ServerApiClient> CreateApiClient()
        {
            SqlTools.GetApiKey(_dbName, out var apiKey, out var apiSecret);
            var apiClient = new ServerApiClient();
            apiClient.Open(new Uri(_serverAddress), apiKey, apiSecret);
            ApplicationId = await apiClient.EnsureApplication("ForTests");
            await apiClient.Reset(ApplicationId, "IntegrationTests");
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