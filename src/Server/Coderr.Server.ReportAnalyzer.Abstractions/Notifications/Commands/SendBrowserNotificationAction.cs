namespace Coderr.Server.ReportAnalyzer.Abstractions.Notifications.Commands
{
    public class SendBrowserNotificationAction
    {
        /// <summary>
        ///     Method name in the service worker script.
        /// </summary>
        public string Action { get; set; }

        public string Title { get; set; }
    }
}