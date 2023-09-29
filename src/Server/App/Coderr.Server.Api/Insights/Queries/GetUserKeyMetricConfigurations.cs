using System;
using DotNetCqs;

namespace Coderr.Server.Api.Insights.Queries
{
    public class GetUserKeyMetricConfigurations : Query<GetUserKeyMetricConfigurations>
    {
        public GetUserKeyMetricConfigurations(int applicationId, int accountId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
            ApplicationId = applicationId;
            AccountId = accountId;
        }

        protected GetUserKeyMetricConfigurations()
        {
        }

        public int AccountId { get; private set; }

        public int ApplicationId { get; private set; }
    }
}