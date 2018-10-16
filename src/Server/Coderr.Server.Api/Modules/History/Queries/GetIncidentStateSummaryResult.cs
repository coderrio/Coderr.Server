namespace Coderr.Server.Api.Modules.History.Queries
{
    public class GetIncidentStateSummaryResult
    {
        public int ClosedCount { get; set; }
        public int NewCount { get; set; }
        public int ReOpenedCount { get; set; }
    }
}