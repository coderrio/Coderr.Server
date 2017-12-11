using System;
using codeRR.Server.App.Modules.Tagging;
using codeRR.Server.App.Modules.Versions.Config;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Versions.Events
{
    /// <summary>
    /// attaches a version tag to the incident
    /// </summary>
    [Component]
    public class AttachVersionTagToIncident : ITagIdentifier
    {
        private readonly IVersionRepository _repository;
        private ConfigurationStore _configStore;

        /// <summary>
        /// Creates a new instance of <see cref="AttachVersionTagToIncident"/>.
        /// </summary>
        /// <param name="repository">repos</param>
        public AttachVersionTagToIncident(IVersionRepository repository, ConfigurationStore configStore)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
            _configStore = configStore;
        }


        private string GetVersionAssemblyName(int applicationId)
        {
            var config = _configStore.Load<ApplicationVersionConfig>();
            return config == null ? null : config.GetAssemblyName(applicationId);
        }



        /// <inheritdoc />
        public void Identify(TagIdentifierContext context)
        {

            var name = GetVersionAssemblyName(context.ApplicationId);
            if (name == null)
                return;

            var version = context.GetPropertyValue("Assemblies", name);
            if (version != null)
                context.AddTag("v" + version, 1);
        }
    }
}