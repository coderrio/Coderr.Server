namespace Coderr.Server.App.Modules.Triggers
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