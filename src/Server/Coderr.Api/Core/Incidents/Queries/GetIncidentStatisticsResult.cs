namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Result for <see cref="GetIncidentStatistics" />.
    /// </summary>
    public class GetIncidentStatisticsResult
    {
        /// <summary>
        ///     Labels (dates)
        /// </summary>
        public string[] Labels { get; set; }


        /// <summary>
        ///     Incident counts per date
        /// </summary>
        public int[] Values { get; set; }
    }
}