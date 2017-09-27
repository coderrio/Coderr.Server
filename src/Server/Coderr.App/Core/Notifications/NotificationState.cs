namespace codeRR.Server.App.Core.Notifications
{
    /// <summary>
    ///     Type of notification to use
    /// </summary>
    public enum NotificationState
    {
        /// <summary>
        ///     Use global setting
        /// </summary>
        UseGlobalSetting,

        /// <summary>
        ///     Do not notify
        /// </summary>
        Disabled,

        /// <summary>
        ///     By cellphone (text message)
        /// </summary>
        Cellphone,

        /// <summary>
        ///     By email
        /// </summary>
        Email
    }
}