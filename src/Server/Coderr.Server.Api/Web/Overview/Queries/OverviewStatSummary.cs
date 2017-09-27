namespace codeRR.Server.Api.Web.Overview.Queries
{
    /// <summary>
    ///     Stats for the last X days, part of <see cref="GetOverviewResult" />.
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