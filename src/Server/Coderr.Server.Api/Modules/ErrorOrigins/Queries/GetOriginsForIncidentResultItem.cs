namespace codeRR.Server.Api.Modules.ErrorOrigins.Queries
{
    /// <summary>
    ///     Item for <see cref="GetOriginsForIncidentResult" />.
    /// </summary>
    public class GetOriginsForIncidentResultItem
    {
        /// <summary>
        ///     Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        ///     Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        ///     Number of error reports that have been received from this incident
        /// </summary>
        public int NumberOfErrorReports { get; set; }
    }
}