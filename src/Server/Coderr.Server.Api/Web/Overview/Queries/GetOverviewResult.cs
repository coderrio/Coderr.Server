namespace codeRR.Server.Api.Web.Overview.Queries
{
    /// <summary>
    ///     Result for <see cref="GetOverview" />.
    /// </summary>
    public class GetOverviewResult
    {
        /// <summary>
        ///     1 = switch to hours for incidents and reports.
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        ///     One collection per application
        /// </summary>
        public GetOverviewApplicationResult[] IncidentsPerApplication { get; set; }

        /// <summary>
        ///     Aggregated summary
        /// </summary>
        public OverviewStatSummary StatSummary { get; set; }

        /// <summary>
        ///     Labels for the time axis (X-axis) in the chart.
        /// </summary>
        public string[] TimeAxisLabels { get; set; }
    }
}