namespace Coderr.Server.App.Modules.Mine
{
    /// <summary>
    ///     A suggestion
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The suggested incident service will enrich the information with incident details.
    ///     </para>
    /// </remarks>
    public class RecommendedIncident
    {
        /// <summary>
        ///     Application that the incident belongs to
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Will be enriched by the suggestion service if left 0
        ///     </para>
        /// </remarks>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Name of the application
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Will be enriched by the suggestion service if left 0
        ///     </para>
        /// </remarks>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     Suggested incident
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        ///     Why this item was suggested.
        /// </summary>
        public string Motivation { get; set; }

        /// <summary>
        ///     Calculated score.
        /// </summary>
        /// <remarks>
        ///     100 points should be distributed between all incidents that a provider recommends.
        /// </remarks>
        public int Score { get; set; }
    }
}