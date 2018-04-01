using System;
using System.Linq;
using System.Reflection;
using Coderr.Server.ReportAnalyzer.Abstractions.Boot;

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