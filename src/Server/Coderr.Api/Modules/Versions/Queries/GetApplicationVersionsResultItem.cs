using System;

namespace codeRR.Server.Api.Modules.Versions.Queries
{
    /// <summary>
    ///     Version information
    /// </summary>
    public class GetApplicationVersionsResultItem
    {
        /// <summary>
        ///     When we received the first incident for this application
        /// </summary>
        public DateTime FirstReportReceivedAtUtc { get; set; }

        /// <summary>
        ///     Number of new incidents
        /// </summary>
        public int IncidentCount { get; set; }

        /// <summary>
        ///     When we received the most recent report for this version
        /// </summary>
        public DateTime LastReportReceivedAtUtc { get; set; }

        /// <summary>
        ///     Number of reports (for old and new incidents)
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        ///     Version string (x.x.x.x)
        /// </summary>
        public string Version { get; set; }
    }
}