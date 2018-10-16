using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coderr.Server.ReportAnalyzer.Abstractions.Boot;
using ConfigurationContext = Coderr.Server.ReportAnalyzer.Abstractions.Boot.ConfigurationContext;
using IConfigurationSection = Coderr.Server.ReportAnalyzer.Abstractions.Boot.IConfigurationSection;
using StartContext = Coderr.Server.ReportAnalyzer.Abstractions.Boot.StartContext;

namespace Coderr.Server.ReportAnalyzer.Boot
{
    public class ModuleStarter
    {
        private readonly List<string> _ignoredModules = new List<string>();
        private readonly List<IReportAnalyzerModule> _modules = new List<IReportAnalyzerModule>();

        public void Configure(ConfigurationContext context, IConfigurationSection ignoredModulesConfigurationSection)
        {
            foreach (var child in ignoredModulesConfigurationSection.GetChildren()) 
                _ignoredModules.Add(child.Value);


            ScanAssembliesForModules(AppDomain.CurrentDomain.BaseDirectory);

            foreach (var module in _modules) 
                module.Configure(context);
        }

        public void Start(StartContext context)
        {
            foreach (var module in _modules) module.Start(context);
        }

        public void Stop()
        {
            foreach (var module in _modules) module.Stop();
        }

        private void RegisterModule(Type type)
        {
            var module = (IReportAnalyzerModule) Activator.CreateInstance(type);
            _modules.Add(module);
        }


        private void ScanAssembliesForModules(string assemblyDirectory)
        {
            var files = Directory.GetFiles(assemblyDirectory, "*.dll");
            foreach (var fullPath in files)
            {
                var fileName = Path.GetFileName(fullPath);
                if (!fileName.Contains("Coderr"))
                    continue;

                var assembly = Assembly.LoadFrom(fullPath);
                var types = assembly.GetTypes()
                    .Where(x => typeof(IReportAnalyzerModule).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
                    .ToList();
                foreach (var type in types)
                {
                    if (_ignoredModules.Any(x => x.Equals(type.Name, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    RegisterModule(type);
                }
            }
        }
    }
}