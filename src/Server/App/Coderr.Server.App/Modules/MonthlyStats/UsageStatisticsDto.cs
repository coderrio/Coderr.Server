using System;

namespace Coderr.Server.App.Modules.MonthlyStats
{
    public class UsageStatisticsDto
    {
        public string InstallationId { get; set; }
        public ApplicationUsageStatisticsDto[] Applications { get; set; }

        public DateTime YearMonth { get; set; }
    }
}