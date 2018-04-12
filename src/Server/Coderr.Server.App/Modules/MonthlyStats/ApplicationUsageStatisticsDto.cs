namespace Coderr.Server.App.Modules.MonthlyStats
{
    public class ApplicationUsageStatisticsDto
    {
        public int ApplicationId { get; set; }
        public int ReportCount { get; set; }
        public int IncidentCount { get; set; }
    }
}