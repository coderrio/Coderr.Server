using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using Coderr.Server.WebPush;
using Coderr.Server.WebPush.Model;
using log4net;
using Newtonsoft.Json;
using NotificationAction = Coderr.Server.WebPush.Model.NotificationAction;

namespace Coderr.Server.WebSite.Infrastructure.WebPush
{
    public class PushClient : IWebPushClient
    {
        private readonly VapidDetails _vapid;
        private ILog _logger = LogManager.GetLogger(typeof(PushClient));
        private DateTime _dateLogged = DateTime.Today;

        public PushClient(IConfiguration<BrowserNotificationConfig> pushConfiguration, IConfiguration<BaseConfiguration> baseConfiguration)
        {
            if (ServerConfig.Instance.IsLive)
            {
                baseConfiguration.Value.SupportEmail = "support@coderr.io";
            }

            if (pushConfiguration.Value.PublicKey == null)
            {
                if (_dateLogged < DateTime.Today)
                {
                    _logger.Debug("got no configuration");
                    _dateLogged = DateTime.Today;
                }

                return;
            }

            _vapid = new VapidDetails("mailto:" + baseConfiguration.Value.SupportEmail, pushConfiguration.Value.PublicKey, pushConfiguration.Value.PrivateKey);
        }

        public async Task SendNotification(BrowserSubscription subscription, Notification notification)
        {
            if (_vapid?.PrivateKey == null)
            {
                _logger.Error("WebPush config is missing keys for accountId " + subscription.AccountId);
                return;
            }

            var dto = new PushSubscription(subscription.Endpoint, subscription.PublicKey,
                subscription.AuthenticationSecret);

            var dtoNotification = new PushNotification(notification.Body)
            {
                Actions = notification.Actions.Select(x => new NotificationAction(x.Action, x.Title)).ToList(),
                Body = notification.Body,
                Title = notification.Title,
                Badge = notification.Badge,
                Data = notification.Data,
                Icon = notification.Icon,
                Image = notification.Image,
                Lang = notification.Lang,
                RequireInteraction = notification.RequireInteraction,
                Tag = notification.Tag,
                Timestamp = notification.Timestamp
            };

            try
            {
                _logger.Debug(
                    $"Sending Push notification using {JsonConvert.SerializeObject(_vapid)} to {JsonConvert.SerializeObject(subscription)}");
                var client = new WebPushClient(_vapid);
                await client.NotifyAsync(dto, dtoNotification);
            }
            catch (WebPushException ex)
            {
                if (ex.Message == "Subscription no longer valid")
                {
                    throw new InvalidSubscriptionException(
                        $"Subscription for {subscription.Endpoint} is no longer valid.", ex);
                }

                throw;
            }
        }
    }
}