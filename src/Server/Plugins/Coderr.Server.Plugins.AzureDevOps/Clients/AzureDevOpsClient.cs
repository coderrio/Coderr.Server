using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace Coderr.Server.Common.AzureDevOps.App.Clients
{
    public class AzureDevOpsClient
    {
        private readonly string _personalAccessToken;
        private readonly string _uri;

        /// <summary>
        ///     Constructor. Manually set values to match your organization.
        /// </summary>
        /// <param name="url">$"https://dev.azure.com/{organizationName}"</param>
        public AzureDevOpsClient(string personalAccessToken, string url)
        {
            _personalAccessToken = personalAccessToken ?? throw new ArgumentNullException(nameof(personalAccessToken));
            _uri = url ?? throw new ArgumentNullException(nameof(url));
        }

        protected WorkItemTrackingHttpClient CreateWitClient(VssConnection connection = null)
        {
            if (connection == null)
                connection = CreateConnection();

            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            return workItemTrackingHttpClient;
        }

        public ProfileHttpClient CreateProfileClient()
        {
            var connection = CreateConnection();

            var client = connection.GetClient<ProfileHttpClient>();
            return client;
        }

        protected VssConnection CreateConnection()
        {
            var uri = new Uri(_uri);
            var credentials = new VssBasicCredential("", _personalAccessToken);
            var connection = new VssConnection(uri, credentials);
            return connection;
        }

    }
}
