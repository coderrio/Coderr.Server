using System.Threading.Tasks;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;

namespace Coderr.Server.ReportAnalyzer.UserNotifications
{
    /// <summary>
    ///     Abstraction for the web push implementation.
    /// </summary>
    public interface IWebPushClient
    {
        /// <summary>
        ///     Send the notification to the push service of the browser.
        /// </summary>
        /// <param name="subscription">User subscription</param>
        /// <param name="notification">Information to send.</param>
        /// <returns></returns>
        Task SendNotification(BrowserSubscription subscription, Notification notification);
    }
}