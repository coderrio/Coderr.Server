namespace codeRR.Server.Api.Modules.Triggers.Queries
{
    /// <summary>
    ///     Result for <see cref="GetTrigger" />
    /// </summary>
    public class GetTriggerDTO
    {
        /// <summary>
        ///     Actions to take if all rules says OK.
        /// </summary>
        public TriggerActionDataDTO[] Actions { get; set; }

        /// <summary>
        ///     Application that the trigger is for.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     What the trigger does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Trigger id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Decision to use if all rules have been passed.
        /// </summary>
        public LastTriggerActionDTO LastTriggerAction { get; set; }

        /// <summary>
        ///     Trigger name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Rules deciding if actions can be run.
        /// </summary>
        public TriggerRuleBase[] Rules { get; set; }

        /// <summary>
        ///     Run for incidents that already has one or more reports.
        /// </summary>
        public bool RunForExistingIncidents { get; set; }

        /// <summary>
        ///     Run trigger for new incidents (got a new unqiue exception)
        /// </summary>
        public bool RunForNewIncidents { get; set; }

        /// <summary>
        ///     Run when a closed incident get its first new report.
        /// </summary>
        public bool RunForReOpenedIncidents { get; set; }
    }
}