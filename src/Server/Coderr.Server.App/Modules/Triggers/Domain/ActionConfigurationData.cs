namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Defines information for a specific action in a trigger.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         "Send email", for instance, might have email address as <see cref="Data" />.
    ///     </para>
    /// </remarks>
    public class ActionConfigurationData
    {
        /// <summary>
        ///     Action to take
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        ///     Context data for the action.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        ///     Primary key
        /// </summary>
        public int Id { get; set; }
    }
}