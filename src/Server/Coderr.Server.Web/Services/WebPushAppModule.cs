using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Web.Services
{
    public class WebPushAppModule : IAppModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Services.AddScoped(typeof(IWebPushClient), typeof(PushClient));
            context.Services.AddSingleton(Startup.Configuration);
        }

        public void Start(StartContext context)
        {
        }

        public void Stop()
        {
        }
    }
}