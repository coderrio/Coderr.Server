namespace Coderr.Server.App.Modules.MonthlyStats
{
    public class ApplicationUsageStatisticsDto
    {
        public int ApplicationId { get; set; }
        public int ReportCount { get; set; }
        public int IncidentCount { get; set; }
        public int ReOpenedCount { get; set; }
        public int ClosedCount { get; set; }
        public int IgnoredCount { get; set; }
        public decimal? NumberOfDevelopers { get; set; }
        public int? EstimatedNumberOfErrors { get; set; }
        public bool IsEmpty => ReportCount == 0 && ClosedCount == 0;
    }
}