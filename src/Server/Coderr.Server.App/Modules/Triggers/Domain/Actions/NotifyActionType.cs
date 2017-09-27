namespace codeRR.Server.App.Modules.Triggers.Domain.Actions
{
    /// <summary>
    ///     When to notify users
    /// </summary>
    public enum NotifyActionType
    {
        /// <summary>
        ///     notify if filter validates to false
        /// </summary>
        NotifyOnFailure,

        /// <summary>
        ///     notify on filter success
        /// </summary>
        NotifyOnSuccess
    }
}