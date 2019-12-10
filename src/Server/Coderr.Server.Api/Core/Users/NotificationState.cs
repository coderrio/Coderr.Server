namespace Coderr.Server.Api.Core.Users
{
    /// <summary>
    ///     Type of notification to use
    /// </summary>
    public enum NotificationState
    {
        /// <summary>
        ///     Use global setting
        /// </summary>
        UseGlobalSetting = 0,

        /// <summary>
        ///     Do not notify
        /// </summary>
        Disabled = 1,

        /// <summary>
        ///     By cellphone (text message)
        /// </summary>
        Cellphone = 2,

        /// <summary>
        ///     By email
        /// </summary>
        Email = 3,

        /// <summary>
        /// Use browser/desktop notifications.
        /// </summary>
        BrowserNotification = 4
    }
}