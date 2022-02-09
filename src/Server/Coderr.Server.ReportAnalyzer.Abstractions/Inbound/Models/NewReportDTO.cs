using System;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Models
{
    /// <summary>
    ///     Report as uploaded by the client API (should always match the client library contracts, but with with own additions like RemoteIp).
    /// </summary>
    public class NewReportDTO
    {
        /// <summary>
        ///     A collection of context information such as HTTP request information or computer hardware info.
        /// </summary>
        public NewReportContextInfo[] ContextCollections { get; set; }

        /// <summary>
        ///     Date specified at client side
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }


        /// <summary>
        ///     Exception which was caught.
        /// </summary>
        public NewReportException Exception { get; set; }

        /// <summary>
        /// The 100 last log entries before the exception was detected (can be null).
        /// </summary>
        public NewReportLogEntry[] LogEntries { get; set; }

        /// <summary>
        /// "Dev", "Production" etc.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Used in older clients.
        /// </summary>
        public string Environment { get; set; }

        public string RemoteAddress { get; set; }

        /// <summary>
        ///     Gets incident id (unique identifier used in communication with the customer to identify this error)
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        ///     Version of the report
        /// </summary>
        public string ReportVersion { get; set; }
    }
}