namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     A suggested solution for the incident
    /// </summary>
    public class SuggestedIncidentSolution
    {
        /// <summary>
        ///     Common reasons to why this exception is thrown.
        /// </summary>
        public string Reason { get; set; }


        /// <summary>
        ///     How the incident can be solved.
        /// </summary>
        public string SuggestedSolution { get; set; }
    }
}