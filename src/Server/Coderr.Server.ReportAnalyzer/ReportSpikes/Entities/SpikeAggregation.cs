using System;

namespace Coderr.Server.ReportAnalyzer.ReportSpikes.Entities
{
    /// <summary>
    ///     Keeps track of the reports for a specific application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The data itself does not indicate a spike, but are used to detect spikes. Once a spike is detected, we notify
    ///         the users and then mark the day as completed (notified)
    ///     </para>
    /// </remarks>
    public class SpikeAggregation
    {
        /// <summary>
        ///     Application that the report count is for.
        /// </summary>
        public int ApplicationId { get; set; }

        public int Id { get; set; }

        /// <summary>
        ///     we have notified peo
        /// </summary>
        public bool Notified { get; set; }

        /// <summary>
        ///     Number of reports received today.
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        /// 50 percentile for the last 7 days.
        /// </summary>
        public int Percentile50 { get; set; }

        /// <summary>
        /// Name of the application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 85th percentile for the last 7 days.
        /// </summary>
        public int Percentile85 { get; set; }

        /// <summary>
        ///     Date that we are storing the report count for.
        /// </summary>
        public DateTime TrackedDate { get; set; }
    }
}