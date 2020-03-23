using System;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using log4net;

namespace Coderr.Server.ReportAnalyzer.UserNotifications
{
    // Must be here so that it can be used from both queues.
    /// <summary>
    /// Implementation of <see cref="INotificationService"/>.
    /// </summary>
    [ContainerService]
    public class NotificationService : INotificationService
    {
        private readonly IWebPushClient _client;
        private readonly ILog _logger = LogManager.GetLogger(typeof(NotificationService));
        private readonly IUserNotificationsRepository _repository;

        public NotificationService(IUserNotificationsRepository repository, IWebPushClient client)
        {
            _repository = repository;
            _client = client;
        }

        public async Task SendBrowserNotification(int accountId, Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));

            var subscriptions = await _repository.GetSubscriptions(accountId);
            foreach (var subscription in subscriptions)
            {
                try
                {
                    await _client.SendNotification(subscription, notification);
                }
                catch (InvalidSubscriptionException e)
                {
                    _logger.Error("Failed to send notification to " + subscription.AccountId, e);
                    await _repository.Delete(subscription);
                }
                catch (Exception e)
                {
                    _logger.Error("Failed to send notification to " + subscription.AccountId, e);
                }
            }
        }
   }
}