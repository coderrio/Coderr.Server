using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Modules.Mine;
using Coderr.Server.Domain.Modules.Tags;

namespace Coderr.Server.App.Tags.Services
{
    [ContainerService]
    class RecommendUsingTags : IRecommendationProvider
    {
        private ITagsRepository _tagsRepository;

        public RecommendUsingTags(ITagsRepository tagsRepository)
        {
            _tagsRepository = tagsRepository;
        }

        public async Task Recommend(RecommendIncidentContext context)
        {
            var importantIncidents = await _tagsRepository.GetNewIncidentsForTag(context.ApplicationId, "important");
            var criticalIncidents = await _tagsRepository.GetNewIncidentsForTag(context.ApplicationId, "critical");
            var itemsLeft = context.NumberOfItems;
            foreach (var incident in criticalIncidents)
            {
                if (itemsLeft-- == 0)
                    break;

                context.Add(new RecommendedIncident
                {
                    IncidentId = incident,
                    Score = 100,
                    Motivation = "Marked as critical"

                }, 10);
            }
            foreach (var incident in importantIncidents)
            {
                if (itemsLeft-- == 0)
                    break;

                context.Add(new RecommendedIncident
                {
                    IncidentId = incident,
                    Score = 90,
                    Motivation = "Marked as important"
                }, 10);
            }
        }
    }
}
