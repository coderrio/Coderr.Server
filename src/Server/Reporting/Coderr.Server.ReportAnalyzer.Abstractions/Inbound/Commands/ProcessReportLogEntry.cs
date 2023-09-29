using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Commands
{
    /// <summary>
    /// Process (analyze) a new error report.
    /// </summary>
    public class ProcessReportLogEntry
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
        /// Where in the code that the log entry is from (class name, method name or similar)
        /// </summary>
        public string Source { get; set; }
    }
}
