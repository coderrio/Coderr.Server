using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Api.Modules.Tagging.Events;
using Coderr.Server.Domain.Modules.Partitions;
using Coderr.Server.Domain.Modules.Tags;
using DotNetCqs;
using Griffin.ApplicationServices;

namespace Coderr.Server.ReportAnalyzer.Partitions.Jobs
{
    [ContainerService(RegisterAsSelf = true)]
    internal class FindIncidentsToTagJob : IBackgroundJobAsync
    {
        private readonly IPartitionRepository _repository;
        private readonly ITagsRepository _tagsRepository;
        private IMessageBus _messageBus;

        public FindIncidentsToTagJob(IPartitionRepository repository, ITagsRepository tagsRepository, IMessageBus messageBus)
        {
            _repository = repository;
            _tagsRepository = tagsRepository;
            _messageBus = messageBus;
        }

        public async Task ExecuteAsync()
        {
            var definitions = await _repository.GetDefinitionsToTag();
            foreach (var definition in definitions)
            {
                var incidents = await _repository.GetIncidentsToTag(definition);
                foreach (var incident in incidents)
                {
                    if (incident.MarkAsCritical)
                    {
                        await _tagsRepository.UpdateTags(incident.IncidentId, new[] {"critical"}, new[] {"important"});
                        await _messageBus.SendAsync(new TagAttachedToIncident(incident.ApplicationId, incident.IncidentId, new[] {"critical"}));
                    }
                    else if (incident.MarkAsImportant)
                    {
                        await _tagsRepository.AddTag(incident.IncidentId, "important");
                        await _messageBus.SendAsync(new TagAttachedToIncident(incident.ApplicationId, incident.IncidentId, new[] {"important"}));
                    }
                }
            }
        }
    }
}