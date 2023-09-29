using System;

namespace Coderr.Server.Api.Modules.Versions.Queries
{
    public class GetVersionHistoryResultItem
    {
        public int IncindentCount { get; set; }
        public int ReportCount { get; set; }
        public DateTime LastUpdateAtUtc { get; set; }
        public string Version { get; set; }
    }
}