using System;
using DotNetCqs;

namespace Coderr.Server.Api.Partitions.Queries
{
    /// <summary>
    /// Fetch all received values for a specific partition
    /// </summary>
    [Message]
    public class GetPartitionValues : Query<GetPartitionValuesResult>
    {
        public GetPartitionValues(int partitionId)
        {
            if (partitionId <= 0) throw new ArgumentOutOfRangeException(nameof(partitionId));
            PartitionId = partitionId;
        }

        /// <summary>
        /// Partition to fetch items for.
        /// </summary>
        public int PartitionId { get; private set; }

        /// <summary>
        /// Limit result to this specific incident.
        /// </summary>
        public int? IncidentId { get; set; }

        /// <summary>
        /// We expect a lot of results. Request a specific result page.
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Only fetch this amount of items.
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
