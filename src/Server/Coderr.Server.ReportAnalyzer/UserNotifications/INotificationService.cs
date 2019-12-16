using System.Threading.Tasks;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;

namespace Coderr.Server.ReportAnalyzer.UserNotifications
{
    /// <summary>
    ///     Used to send notifications to users.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        ///     Send a browser notification (requires that the user first have approved notifications through javascript).
        /// </summary>
        /// <param name="accountId">Account to send to</param>
        /// <param name="notification">Notification details</param>
        /// <returns></returns>
        Task SendBrowserNotification(int accountId, Notification notification);
    }
}