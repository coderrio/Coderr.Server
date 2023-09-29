namespace Coderr.Server.Api.Insights.Queries
{
    public class MetricToplistItem
    {
        public int Id { get; set; }
        public object Value { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
    }
}