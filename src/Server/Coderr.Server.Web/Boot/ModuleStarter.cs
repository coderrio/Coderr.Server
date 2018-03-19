using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.Web2.Boot.Adapters;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Coderr.Server.Web2.Boot
{
    public class ModuleStarter
    {
        private readonly List<ISystemModule> _modules = new List<ISystemModule>();
        private ConfigurationWrapper _configurationWrapper;

        public ModuleStarter(IConfiguration configuration)
        {
            _configurationWrapper = new ConfigurationWrapper(configuration);
        }

        public void RegisterModule(Type type)
        {
            var module = (ISystemModule)Activator.CreateInstance(type);
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
                    .Where(x => typeof(ISystemModule).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
                    .ToList();
                foreach (var type in types)
                {
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