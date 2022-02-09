using System;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Models
{
    /// <summary>
    /// Logentry for <see cref="NewReportDTO"/>.
    /// </summary>
    public class NewReportLogEntry
    {

        /// <summary>
        /// When this log entry was written
        /// </summary>
        public DateTime TimestampUtc { get; set; }

        /// <summary>
        /// 0 = trace, 1 = debug, 2 = info, 3 = warning, 4 = error, 5 = critical
        /// </summary>
        public int LogLevel { get; set; }

        /// <summary>
        /// Logged message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception as string (if any was attached to this log entry)
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Location in the code that generated this log entry. Can be null.
        /// </summary>
        public string Source { get; set; }
    }
}