using System;
using Coderr.Server.Api;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    [Message]
    public class GetAzureSettings : Query<GetAzureSettingsResult>
    {
        public GetAzureSettings(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            ApplicationId = applicationId;
        }

        protected GetAzureSettings()
        {
        }

        public int ApplicationId { get; private set; }
    }
}