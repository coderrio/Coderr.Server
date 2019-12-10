namespace Coderr.Server.Domain.Modules.UserNotifications
{
    /// <summary>
    ///     Type of notification to use
    /// </summary>
    public enum NotificationState
    {
        /// <summary>
        ///     Use global setting
        /// </summary>
        UseGlobalSetting = 1,

        /// <summary>
        ///     Do not notify
        /// </summary>
        Disabled = 2,

        /// <summary>
        ///     By cellphone (text message)
        /// </summary>
        Cellphone = 3,

        /// <summary>
        ///     By email
        /// </summary>
        Email = 4
    }
}