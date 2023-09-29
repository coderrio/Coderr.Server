using System.Collections.Generic;
using System.Security.Claims;

namespace Coderr.Server.App.Insights.Metrics
{
    public class KeyMetricGeneratorContext
    {
        public ClaimsPrincipal User { get; set; }

        public int? ApplicationId { get; set; }

        public IDictionary<int, string> ApplicationNames { get; set; }

        public int DaysFrom { get; set; }
        public int DaysTo { get; set; }

        /// <summary>
        /// Generate a top 10 list.
        /// </summary>
        public bool IncludeTopList { get; set; }

        /// <summary>
        /// Generate a chart.
        /// </summary>
        public bool IncludeChartData { get; set; }

        /// <summary>
        /// Calculate the summary/count.
        /// </summary>
        public bool IncludeMetric { get; set; }
    }
}