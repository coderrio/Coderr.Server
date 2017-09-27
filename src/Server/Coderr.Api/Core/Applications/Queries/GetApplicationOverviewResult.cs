namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Result for <see cref="GetApplicationOverview" />.
    /// </summary>
    //TODO, move to the web namespace.
    public class GetApplicationOverviewResult
    {
        /// <summary>
        ///     1 = switch to hours for incidents and reports.
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        ///     One entry for each day
        /// </summary>
        public int[] ErrorReports { get; set; }

        /// <summary>
        ///     One incident count for each day
        /// </summary>
        public int[] Incidents { get; set; }

        /// <summary>
        ///     Statistics summary
        /// </summary>
        public OverviewStatSummary StatSummary { get; set; }

        /// <summary>
        ///     Labels for X axis
        /// </summary>
        public string[] TimeAxisLabels { get; set; }
    }
}