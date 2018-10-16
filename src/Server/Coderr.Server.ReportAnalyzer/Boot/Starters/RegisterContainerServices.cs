using System;
using System.Linq;
using System.Reflection;
using Coderr.Server.ReportAnalyzer.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Inbound.Handlers;

namespace Coderr.Server.ReportAnalyzer.Boot.Starters
{
    public class RegisterContainerServices : IReportAnalyzerModule
    {
        public void Start(StartContext context)
        {
        }

        public void Configure(ConfigurationContext context)
        {
            context.Services.RegisterContainerServices(Assembly.GetExecutingAssembly());
            context.Services.RegisterContainerServices(typeof(ProcessReportHandler).Assembly);

            //workaround since SQL server already references us
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .First(x => x.FullName.StartsWith("Coderr.Server.SqlServer,"));
            context.Services.RegisterContainerServices(assembly);
        }

        public void Stop()
        {
        }
    }
}