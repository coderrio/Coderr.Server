using System;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Commands
{
    public class StoreLogEntriesEntry
    {
        public DateTime TimeStampUtc { get; set; }

        public string Message { get; set; }

        public StoreLogEntriesLogLevel Level { get; set; }

        public string Exception { get; set; }

        public string Source { get; set; }
    }
}