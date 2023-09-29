namespace Coderr.Server.App.Insights.Metrics
{
    public class Metric
    {
        public bool IsAlternative { get; set; }

        /// <summary>
        /// Comment explaining the value (like, "best application")
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Actual value.
        /// </summary>
        public object Value { get; set; }
    }
}