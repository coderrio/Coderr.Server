using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.ReportAnalyzer.Abstractions.Notifications.Commands;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using DotNetCqs;

namespace Coderr.Server.ReportAnalyzer.UserNotifications.Handlers
{
    internal class SendBrowserNotificationHandler : IMessageHandler<SendBrowserNotification>
    {
        private readonly INotificationService _notificationService;

        public SendBrowserNotificationHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(IMessageContext context, SendBrowserNotification message)
        {
            var notification = new Notification
            {
                Actions = message.Actions.Select(ConvertAction).ToList(),
                Badge = message.Badge,
                Body = message.Body,
                Data = message.UserData,
                Icon = message.IconUrl,
                Image = message.ImageUrl,
                Lang = message.LanguageCode,
                RequireInteraction = message.RequireInteraction,
                Tag = message.Tag,
                Timestamp = message.Timestamp,
                Title = message.Title
            };
            await _notificationService.SendBrowserNotification(message.AccountIdToSendTo, notification);
        }

        private NotificationAction ConvertAction(SendBrowserNotificationAction arg)
        {
            return new NotificationAction {Title = arg.Title, Action = arg.Action};
        }
    }
}