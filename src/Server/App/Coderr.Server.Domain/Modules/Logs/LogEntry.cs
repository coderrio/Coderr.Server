using System;

namespace Coderr.Server.Domain.Modules.Logs
{
    public class LogEntry
    {
        public DateTime TimeStampUtc { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }

        public string Exception { get; set; }

        /// <summary>
        /// Class name, method or similar (i.e. where in the code that this log entry comes from)
        /// </summary>
        public string Source { get; set; }
    }
}