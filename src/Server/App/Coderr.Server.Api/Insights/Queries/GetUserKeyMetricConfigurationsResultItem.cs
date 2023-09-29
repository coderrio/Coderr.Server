namespace Coderr.Server.Api.Insights.Queries
{
    public class GetUserKeyMetricConfigurationsResultItem
    {
        public int DaysFrom { get; set; }
        public int DaysTo { get; set; }
        public int Id { get; set; }

        public int MetricId { get; set; }
    }
}