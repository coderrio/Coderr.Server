using System;
using DotNetCqs;

namespace Coderr.Server.Api.Partitions.Queries
{
    /// <summary>
    ///     Get insights for partitions.
    /// </summary>
    [Message]
    public class GetPartitionInsights : Query<GetPartitionInsightsResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GetPartitionInsights" /> class.
        /// </summary>
        /// <param name="applicationId">Application to fetch partitions for.</param>
        public GetPartitionInsights(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            ApplicationIds = new[] {applicationId};
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GetPartitionInsights" /> class.
        /// </summary>
        /// <param name="applicationIds">Applications to fetch partitions for.</param>
        public GetPartitionInsights(int[] applicationIds)
        {
            if (applicationIds == null || applicationIds.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(applicationIds));
            ApplicationIds = applicationIds;
        }

        protected GetPartitionInsights()
        {
        }

        /// <summary>
        ///     Applications to load insights for.
        /// </summary>
        public int[] ApplicationIds { get; private set; }

        /// <summary>
        /// Limit to a specific error.
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        /// Collect stats from this day and forwards.
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-11);

        /// <summary>
        /// Collect stats to this date.
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Used to summarize a certain period (like the last 90 days).
        /// </summary>
        public DateTime SummarizePeriodStartDate { get; set; } = DateTime.Today.AddDays(-30);

        /// <summary>
        /// Used to summarize a certain period (like the last 90 days).
        /// </summary>
        public DateTime SummarizePeriodEndDate { get; set; } = DateTime.UtcNow;
        
    }
}