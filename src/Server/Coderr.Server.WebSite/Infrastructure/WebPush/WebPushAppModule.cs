using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.WebSite.Infrastructure.WebPush
{
    public class WebPushAppModule : IAppModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Services.AddScoped(typeof(IWebPushClient), typeof(PushClient));
        }

        public void Start(StartContext context)
        {
        }

        public void Stop()
        {
        }
    }
}