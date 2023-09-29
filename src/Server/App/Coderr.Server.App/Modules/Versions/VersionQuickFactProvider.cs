using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Domain.Modules.ApplicationVersions;

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

        public async Task CollectAsync(QuickFactContext context)
        {
            var versions = await _repository.FindForIncidentAsync(context.IncidentId);
            if (!versions.Any())
            {
                return;
            }

            context.CollectedFacts.Add(new QuickFact
            {
                Title = "Versions",
                Description = "Application versions that this incident have been detected in.",
                Value = string.Join(", ", versions.Select(x => "v" + x.Version))
            });
        }
    }
}
