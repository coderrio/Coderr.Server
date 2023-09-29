using System;
using DotNetCqs;

namespace Coderr.Server.Api.Partitions.Queries
{
    [Message]
    public class GetPartitions : Query<GetPartitionsResult>
    {
        public GetPartitions(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            ApplicationId = applicationId;
        }

        protected GetPartitions()
        {

        }

        public int ApplicationId { get; private set; }
    }
}

