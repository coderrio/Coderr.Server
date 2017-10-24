using DotNetCqs;

namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Get statistics (i.e. history for a certain period of time)
    /// </summary>
    [Message]
    public class GetIncidentStatistics : Query<GetIncidentStatisticsResult>
    {
        /// <summary>
        ///     Incident to get stats for.
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        ///     Amount of time to look back (i.e. start date = DateTime.Now.Substract(WindowSize))
        /// </summary>
        /// <remarks>
        ///     1 = switch to hours
        /// </remarks>
        public int NumberOfDays { get; set; }
    }
}