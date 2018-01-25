using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Queries;
using Coderr.Server.PluginApi;
using Coderr.Server.PluginApi.Incidents;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Versions
{
    [Component]
    class VersionQuickFactProvider : IQuickfactProvider
    {
        private IVersionRepository _repository;

        public VersionQuickFactProvider(IVersionRepository repository)
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
