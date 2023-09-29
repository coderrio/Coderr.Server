namespace Coderr.Server.Api.WorkItems.Queries
{
    /// <summary>
    ///     Result for <see cref="FindIntegration" />
    /// </summary>
    public class FindIntegrationResult

    {
        /// <summary>
        ///     We have an integration
        /// </summary>
        public bool HaveIntegration { get; set; }


        /// <summary>
        ///     Technical name used to identify it internally
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Human friendly title.
        /// </summary>
        public string Title { get; set; }
    }
}