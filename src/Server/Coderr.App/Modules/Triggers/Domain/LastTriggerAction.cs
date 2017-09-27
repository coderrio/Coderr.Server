namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     What to do if all filter rules have accepted the report.
    /// </summary>
    public enum LastTriggerAction
    {
        /// <summary>
        ///     Grant actions execution.
        /// </summary>
        Grant,

        /// <summary>
        ///     Abort.
        /// </summary>
        Revoke
    }
}