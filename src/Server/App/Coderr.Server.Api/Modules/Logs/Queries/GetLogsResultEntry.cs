using System;

namespace Coderr.Server.Api.Modules.Logs.Queries
{
    public class GetLogsResultEntry
    {
        public DateTime TimeStampUtc { get; set; }

        public string Message { get; set; }

        public GetLogsResultEntryLevel Level { get; set; }

        public string Exception { get; set; }
    }
}