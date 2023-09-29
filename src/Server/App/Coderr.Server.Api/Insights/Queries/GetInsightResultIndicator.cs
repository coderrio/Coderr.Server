namespace Coderr.Server.Api.Insights.Queries
{
    public class GetInsightResultIndicator
    {
        public string PeriodValueName { get; set; }

        public string Title { get; set; }
        public bool IsAlternative { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }

        public object Value { get; set; }
        public bool CanBeNormalized { get; set; }

        public string ValueName { get; set; }

        public object PeriodValue { get; set; }

        public TrendLine[] TrendLines { get; set; }

        public bool? HigherValueIsBetter { get; set; }

        public string ValueUnit { get; set; }
        public string Name { get; set; }
        public IndicatorToplistItem[] Toplist { get; set; }
        public bool IsValueNameApplicationName { get; set; }
    }

    public class IndicatorToplistItem
    {
        public object Value { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
    }
}