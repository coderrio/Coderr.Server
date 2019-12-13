using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Core.Notifications;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebPush;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Coderr.Server.Web.Services
{
    [ContainerService(IsSingleInstance = true)]
    public class PushClient : IWebPushClient
    {
        private readonly WebPushClient _client = new WebPushClient();
        private readonly VapidDetails _vapid;

        public PushClient(IConfiguration configuration)
        {
            var subject = configuration.GetValue<string>("Vapid:Subject");
            var key = configuration.GetValue<string>("Vapid:PublicKey");
            var secret = configuration.GetValue<string>("Vapid:PrivateKey");
            _vapid = new VapidDetails(subject, key, secret);
        }

        public async Task SendNotification(BrowserSubscription subscription, Notification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var dto = new PushSubscription(subscription.Endpoint, subscription.PublicKey,
                subscription.AuthenticationSecret);
            try
            {
                await _client.SendNotificationAsync(dto, json, _vapid);
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