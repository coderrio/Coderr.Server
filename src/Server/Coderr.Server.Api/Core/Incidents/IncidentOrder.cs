namespace codeRR.Server.Api.Core.Incidents
{
    /// <summary>
    ///     How incidents should be ordered in a list
    /// </summary>
    public enum IncidentOrder
    {
        /// <summary>
        ///     Newest incidents first
        /// </summary>
        Newest,

        /// <summary>
        ///     The incident with the highest number of reports
        /// </summary>
        MostReports,

        /// <summary>
        ///     The incidents with the most given feedback
        /// </summary>
        MostFeedback
    }
}