namespace codeRR.Server.App.Modules.Geolocation
{
    /// <summary>
    ///     List result item for repository queries
    /// </summary>
    public class ErrorOrginListItem
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