using System;

namespace Coderr.Server.App.Partitions.Queries
{
    public class GetUserPartitionConfigurations
    {
        public GetUserPartitionConfigurations(int applicationId, int accountId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
            ApplicationId = applicationId;
            AccountId = accountId;
        }

        protected GetUserPartitionConfigurations()
        {
        }

        public int AccountId { get; private set; }

        public int ApplicationId { get; private set; }
    }
}