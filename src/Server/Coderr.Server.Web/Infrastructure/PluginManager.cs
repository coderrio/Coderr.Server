using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using codeRR.Server.Infrastructure.Plugins;
using log4net;

namespace codeRR.Server.Web.Infrastructure
{
    /// <summary>
    ///     Loads all plugins
    /// </summary>
    public class PluginManager
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(PluginManager));
        private readonly IList<IPlugin> _plugins = new List<IPlugin>();

        public void ConfigurePlugins(Configuration configuration)
        {
            foreach (var plugin in _plugins)
                plugin.Configure(configuration);
        }

        public void Load(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory))
                Directory.CreateDirectory(pluginDirectory);

            var files = Directory.GetFiles(pluginDirectory, "*.dll");
            foreach (var file in files)
            {
                _logger.Debug("Loading " + file);
                var name = AssemblyName.GetAssemblyName(file);
                var assembly = Assembly.Load(name);
                _logger.Debug("Creating plugin type for  " + file);
                var plugin = CreatePluginType(assembly);
                if (plugin == null)
                    throw new InvalidOperationException("Failed to create IPlugin from " + file);
                _plugins.Add(plugin);
            }
        }

        public void PreloadPlugins()
        {
            foreach (var plugin in _plugins)
                plugin.Preload();
        }

        private IPlugin CreatePluginType(Assembly assembly)
        {
            var pluginType = (from t in assembly.GetTypes()
                where typeof(IPlugin).IsAssignableFrom(t)
                      && !t.IsAbstract
                      && !t.IsInterface
                select t).FirstOrDefault();

            if (pluginType == null)
                return null;

            return (IPlugin) Activator.CreateInstance(pluginType);
        }
    }
}