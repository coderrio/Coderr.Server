using System;
using System.Collections.Generic;
using System.Linq;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.App.Modules.Versions.Config
{
    /// <summary>
    ///     Configuration for assembly versions
    /// </summary>
    public class ApplicationVersionConfig : IConfigurationSection
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ApplicationVersionConfig" />.
        /// </summary>
        public ApplicationVersionConfig()
        {
            Items = new List<ApplicationVersionConfigItem>();
        }

        /// <summary>
        /// configurations for the applications
        /// </summary>
        public List<ApplicationVersionConfigItem> Items { get; private set; }

        /// <summary>
        /// "ApplicationVersions"
        /// </summary>
        public string SectionName
        {
            get { return "ApplicationVersions"; }
        }

        /// <inheritdoc />
        public void Load(IDictionary<string, string> settings)
        {
            foreach (var setting in settings)
            {
                if (!int.TryParse(setting.Key.Remove(0, 3), out int id))
                    continue;
                Items.Add(new ApplicationVersionConfigItem(id, setting.Value));
            }
        }

        /// <inheritdoc />
        public IDictionary<string, string> ToDictionary()
        {
            var d = Items.ToDictionary(x => "App" + x.ApplicationId, x => x.AssemblyName);
            return d;
        }

        /// <summary>
        ///     Specify an assembly for an application. There may or may not exist a previously configured value.
        /// </summary>
        /// <param name="applicationId">application to configure</param>
        /// <param name="assemblyName">assembly</param>
        public void AddOrUpdate(int applicationId, string assemblyName)
        {
            if (assemblyName == null) throw new ArgumentNullException(nameof(assemblyName));
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));

            var item = Items.FirstOrDefault(x => x.ApplicationId == applicationId);
            if (item != null)
                item.ChangeAssembly(assemblyName);
            else
                Items.Add(new ApplicationVersionConfigItem(applicationId, assemblyName));
        }

        /// <summary>
        ///     Find if the specified application have been configured.
        /// </summary>
        /// <param name="applicationId">application to get config for</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        /// <returns>name if found; otherwise <c>null</c>.</returns>
        public string GetAssemblyName(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));

            var app = Items.FirstOrDefault(x => x.ApplicationId == applicationId);
            return app?.AssemblyName;
        }
    }
}