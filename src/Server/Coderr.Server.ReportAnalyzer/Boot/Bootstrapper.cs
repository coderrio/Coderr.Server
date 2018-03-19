using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
using Coderr.Server.ReportAnalyzer.Boot.Starters;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot
{
    public class Bootstrapper
    {
        private ReportQueueModule _reportQueueModule;
        
        public void ConfigureContainer(ConfigurationContext context)
        {
            _reportQueueModule = new ReportQueueModule();
            _reportQueueModule.Configure(context);

            context.Services.AddScoped(provider =>
            {
                var principal = provider.GetService<ScopedPrincipal>().Principal;
                return context.ConnectionFactory(principal);
            });

        }
    }
}
