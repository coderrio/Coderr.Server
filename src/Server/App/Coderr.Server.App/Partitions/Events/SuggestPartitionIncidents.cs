using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Modules.Mine;
using Coderr.Server.Domain.Modules.Partitions;

namespace Coderr.Server.App.Partitions.Events
{
    [ContainerService]
    public class SuggestPartitionIncidents : IRecommendationProvider
    {
        private readonly IPartitionRepository _partitionRepository;

        public SuggestPartitionIncidents(IPartitionRepository partitionRepository)
        {
            _partitionRepository = partitionRepository;
        }

        public async Task Recommend(RecommendIncidentContext context)
        {
            var items = context.ApplicationId == null
                ? await _partitionRepository.GetPrioritized()
                : await _partitionRepository.GetPrioritized(context.ApplicationId.Value);

            foreach (var item in items)
            {
                var name = GetNiceName(item.PartitionTitle);
                var pluralName = name.Pluralize().ToLower();

                var percentage = item.ValueCount * 100 / item.TotalCount;
                string motivation;
                if (item.ValueCount == item.TotalCount)
                    motivation = $"Affects all of your {pluralName}";
                else
                    motivation = item.ValueCount == 1
                        ? $"Affects one {name.ToLower()}"
                        : $"Affects {percentage}% of your {pluralName}";

                var suggestion = new RecommendedIncident
                {
                    IncidentId = item.IncidentId,
                    Motivation = motivation,
                    Score = item.Severity
                    // But why?!
                    //Score = item.Severity < 10
                    //    ? item.Severity * percentage
                    //    : (int)(item.Severity * (percentage / 100d))
                };

                context.Add(suggestion, item.Severity);
            }
        }

        private string GetNiceName(string partitionTitle)
        {
            if (partitionTitle.EndsWith("Id"))
                partitionTitle = partitionTitle.Substring(0, partitionTitle.Length - 2);
            return partitionTitle;
        }
    }
}