using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Domain.Modules.ApplicationVersions;
using Coderr.Server.PluginApi.Incidents;
using Griffin.Container;

namespace Coderr.Server.App.Modules.Versions
{
    [ContainerService]
    class VersionQuickFactProvider : IQuickfactProvider
    {
        private IApplicationVersionRepository _repository;

        public VersionQuickFactProvider(IApplicationVersionRepository repository)
        {
            _repository = repository;
        }

        public async Task AssignAsync(int incidentId, ICollection<QuickFact> facts)
        {
            var versions = await _repository.FindForIncidentAsync(incidentId);
            if (!versions.Any())
            {
                return;
            }

            facts.Add(new QuickFact
            {
                Title = "Versions",
                Description = "Application versions that this incident have been detected in.",
                Value = string.Join(", ", versions.Select(x => "v" + x.Version))
            });
        }
    }
}
