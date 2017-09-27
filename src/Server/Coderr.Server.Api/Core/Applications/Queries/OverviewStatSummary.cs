namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Stats for the last seven days
    /// </summary>
    public class OverviewStatSummary
    {
        /// <summary>
        ///     Number of followers
        /// </summary>
        public int Followers { get; set; }

        /// <summary>
        ///     Number of incidents
        /// </summary>
        public int Incidents { get; set; }

        /// <summary>
        ///     Number of reports received
        /// </summary>
        public int Reports { get; set; }

        /// <summary>
        ///     Number user feedback items
        /// </summary>
        public int UserFeedback { get; set; }
    }
}