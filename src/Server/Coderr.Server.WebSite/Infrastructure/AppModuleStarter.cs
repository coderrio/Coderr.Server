using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Boot;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationContext = Coderr.Server.Abstractions.Boot.ConfigurationContext;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using StartContext = Coderr.Server.Abstractions.Boot.StartContext;

namespace Coderr.Server.WebSite.Infrastructure
{
    public class AppModuleStarter
    {
        private readonly List<IAppModule> _modules = new List<IAppModule>();
        private readonly List<string> _ignoredAppModules = new List<string>();
        private ILog _logger = LogManager.GetLogger(typeof(ReportAnalyzerModuleStarter));

        public AppModuleStarter(IConfiguration configuration)
        {
            foreach (var child in configuration.GetSection("DisabledModules:App").GetChildren())
            {
                _ignoredAppModules.Add(child.Value);
            }
           
        }

        public void RegisterModule(Type type)
        {
            var module = (IAppModule)Activator.CreateInstance(type);
            _modules.Add(module);
        }

        public void ScanAssemblies(string assemblyDirectory)
        {
            var files = Directory.GetFiles(assemblyDirectory, "*.dll");
            foreach (var fullPath in files)
            {
                var fileName = Path.GetFileName(fullPath);
                if (!fileName.Contains("Coderr"))
                    continue;


                var assembly = Assembly.LoadFrom(fullPath);
                var types = assembly.GetTypes()
                    .Where(x => typeof(IAppModule).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
                    .ToList();
                foreach (var type in types)
                {
                    if (_ignoredAppModules.Any(x => x.Equals(type.Name, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    if (ServerConfig.Instance.IsModuleIgnored(type))
                        continue;

                    RegisterModule(type);
                }

             
            }
        }

        public void Configure(ConfigurationContext context)
        {
            foreach (var module in _modules)
            {

                var childServices = new ServiceCollection();
                var moduleContext = context.Clone(childServices);

                module.Configure(moduleContext);


                foreach (var service in moduleContext.Services)
                {
                    if (service.ImplementationType?.Name == "DeleteAbandonedSimilarities")
                        Debugger.Break();

                    //if (context.Services.Any(x => x.ImplementationType == service.ImplementationType))
                    //{
                    //    throw new InvalidOperationException(
                    //        $"Service {service.ImplementationType} has already been registered.");
                    //}

                    context.Services.Add(service);
                }
            }
        }

        public void Start(StartContext context)
        {
            foreach (var module in _modules)
            {
                module.Start(context);
            }
        }

        public void Stop()
        {
            foreach (var module in _modules)
            {
                module.Stop();
            }
        }
    }
}