using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using Newtonsoft.Json;
using WebPush;

namespace Coderr.Server.Web.Services
{
    [ContainerService(IsSingleInstance = true)]
    public class PushClient : IWebPushClient
    {
        private readonly WebPushClient _client = new WebPushClient();
        private readonly VapidDetails _vapid;

        public PushClient(IConfiguration<BrowserNotificationConfig> pushConfiguration, IConfiguration<BaseConfiguration> baseConfiguration)
        {
            _vapid = new VapidDetails("mailto:" + baseConfiguration.Value.SupportEmail, pushConfiguration.Value.PublicKey, pushConfiguration.Value.PrivateKey);
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