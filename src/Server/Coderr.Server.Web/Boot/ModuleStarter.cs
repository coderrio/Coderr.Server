using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Web.Boot.Adapters;
using ConfigurationContext = Coderr.Server.Abstractions.Boot.ConfigurationContext;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using StartContext = Coderr.Server.Abstractions.Boot.StartContext;

namespace Coderr.Server.Web.Boot
{
    public class ModuleStarter
    {
        private readonly List<IAppModule> _modules = new List<IAppModule>();
        private ConfigurationWrapper _configurationWrapper;
        private List<string> _ignoredAppModules = new List<string>();

        public ModuleStarter(IConfiguration configuration)
        {
            _configurationWrapper = new ConfigurationWrapper(configuration);
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

                    RegisterModule(type);
                }

             
            }
        }

        public void Configure(ConfigurationContext context)
        {
            foreach (var module in _modules)
            {
                module.Configure(context);
            }
        }

        public void Start(StartContext context)
        {
            foreach (var module in _modules)
            {
                module.Start(context);
            }
        }
    }
}