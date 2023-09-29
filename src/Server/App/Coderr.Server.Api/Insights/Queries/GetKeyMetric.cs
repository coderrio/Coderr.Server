using DotNetCqs;

namespace Coderr.Server.Api.Insights.Queries
{
    public class GetKeyMetric : Query<GetKeyMetricResult>
    {
        public int? ApplicationId { get; set; }

        public int DaysFrom { get; set; }
        public int DaysTo { get; set; }

        public int MetricId { get; set; }

        public bool IncludeTopList { get; set; }
        public bool IncludeChartData { get; set; }

    }
}