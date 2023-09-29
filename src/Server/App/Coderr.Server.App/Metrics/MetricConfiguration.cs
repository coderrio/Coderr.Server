namespace Coderr.Server.App.Metrics
{
    /// <summary>
    /// Configuration of an metric.
    /// </summary>
    /// <remarks>
    ///<para>
    ///Configured per application or system wide. An application specific configuration is not required, if none is specified, the system will take all global metric and apply application ids on them.
    /// </para>
    /// </remarks>
    public class MetricConfiguration
    {
        /// <summary>
        /// Name from <see cref="MetricAttribute"/> that the metric in question was tagged with.
        /// </summary>
        public string MetricName { get; set; }

        /// <summary>
        /// If specified, override the application ids in <see cref="ApplicationIds"/> and generate only for this application.
        /// </summary>
        public int? ConfiguredApplicationId { get; set; }

        /// <summary>
        /// Application group that we should generate metrics for.
        /// </summary>
        /// <remarks>
        ///Field is used to generate all application ids in that group that the user have access to.
        /// </remarks>
        public int GroupId { get; set; }

        /// <summary>
        /// Applications that should be generated in this config.
        /// </summary>
        public int[] ApplicationIds { get; set; }

        /// <summary>
        /// Create one aggregated value for all apps.
        /// </summary>
        /// <remarks>
        ///<para>
        ///If false, return one metric per application.
        /// </para>
        /// </remarks>
        public bool SummarizeApplications { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeDefinition Period { get; set; }

        /// <summary>
        /// Generate a metric value.
        /// </summary>
        public bool GenerateMetric { get; set; }

        /// <summary>
        /// Generate a chart.
        /// </summary>
        public bool GenerateChart { get; set; }
    }
}
