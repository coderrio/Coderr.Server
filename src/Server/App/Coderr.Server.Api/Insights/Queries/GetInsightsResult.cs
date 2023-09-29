namespace Coderr.Server.Api.Insights.Queries
{
    public class GetInsightsResult
    {
        /// <summary>
        /// Global indicators
        /// </summary>
        public GetInsightResultIndicator[] Indicators { get; set; }

        public GetInsightResultApplication[] ApplicationInsights { get; set; }

        /// <summary>
        /// YYYY-MM-DD array
        /// </summary>
        public string[] TrendDates { get; set; }
    }
}