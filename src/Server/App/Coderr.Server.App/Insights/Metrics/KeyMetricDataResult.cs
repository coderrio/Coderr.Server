namespace Coderr.Server.App.Insights.Metrics
{
    public class KeyMetricDataResult
    {
        public KeyMetricDataResultItem Application { get; set; }

        /// <summary>
        /// i.e. average for all apps that the user have access to.
        /// </summary>
        public KeyMetricDataResultItem Global { get; set; }
    }
}