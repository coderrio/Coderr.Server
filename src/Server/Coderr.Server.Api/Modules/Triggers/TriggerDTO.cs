namespace codeRR.Server.Api.Modules.Triggers
{
    /// <summary>
    ///     Trigger DTO
    /// </summary>
    public class TriggerDTO
    {
        /// <summary>
        ///     Description (typically why it was created and what it should do)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Identity
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Trigger name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Short summary
        /// </summary>
        public string Summary { get; set; }
    }
}