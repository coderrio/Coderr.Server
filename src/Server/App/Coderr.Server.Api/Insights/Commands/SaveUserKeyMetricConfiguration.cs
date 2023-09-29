namespace Coderr.Server.Api.Insights.Commands
{
    [Command]
    public class SaveUserKeyMetricConfiguration
    {
        public int DaysFrom { get; set; }
        public int DaysTo { get; set; }
        public int? Id { get; set; }

        public int MetricId { get; set; }
    }
}