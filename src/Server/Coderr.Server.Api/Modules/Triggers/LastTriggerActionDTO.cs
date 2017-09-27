namespace codeRR.Server.Api.Modules.Triggers
{
    /// <summary>
    ///     What to do if all rules accepted the report.
    /// </summary>
    public enum LastTriggerActionDTO
    {
        /// <summary>
        ///     Execute trigger actions.
        /// </summary>
        ExecuteActions,

        /// <summary>
        ///     Abort the trigger
        /// </summary>
        AbortTrigger
    }
}