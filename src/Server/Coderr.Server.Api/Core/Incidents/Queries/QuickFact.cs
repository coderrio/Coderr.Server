namespace codeRR.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Quick fact for incidents.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For instance number of reports, when the incident was created, number of affected users etc.
    ///     </para>
    /// </remarks>
    public class QuickFact
    {
        /// <summary>
        ///     what this fact displays
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Fact title (heading)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Optional url to get more information.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Value to show
        /// </summary>
        /// <remarks>
        ///     For multiple values; separate them with semi colons.
        /// </remarks>
        public string Value { get; set; }
    }
}