namespace Coderr.Server.Api.Insights.Queries
{
    public class GetKeyMetricResultItem
    {
        public MetricToplistItem[] Toplist { get; set; }
        public TrendLine[] ChartData { get; set; }

        public string Title { get; set; }
        public bool IsAlternative { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }

        public object Value { get; set; }
        public bool CanBeNormalized { get; set; }

        public string ValueName { get; set; }

        public bool? HigherValueIsBetter { get; set; }

        public string ValueUnit { get; set; }
        public string Name { get; set; }
        public bool IsValueNameApplicationName { get; set; }
    }
}