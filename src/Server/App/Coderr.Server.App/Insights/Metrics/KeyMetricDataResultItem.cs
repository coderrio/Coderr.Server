using Coderr.Server.Api.Insights.Queries;

namespace Coderr.Server.App.Insights.Metrics
{
    public class KeyMetricDataResultItem
    {
        public MetricToplistItem[] Toplist { get; set; }
        public TrendLine[] ChartData { get; set; }

        public Metric Metric { get; set; }
    }
}