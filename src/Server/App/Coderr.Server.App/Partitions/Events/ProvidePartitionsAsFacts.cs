using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Domain.Modules.Partitions;

namespace Coderr.Server.App.Partitions.Events
{
    [ContainerService]
    public class ProvidePartitionsAsFacts : IQuickfactProvider
    {
        private readonly IPartitionRepository _repository;

        public ProvidePartitionsAsFacts(IPartitionRepository repository)
        {
            _repository = repository;
        }

        public async Task CollectAsync(QuickFactContext context)
        {
            var partitions = await _repository.GetIncidentSummary(context.ApplicationId, context.IncidentId);
            foreach (var partition in partitions)
            {
                if (partition.PartitionTitle.EndsWith("Id"))
                    partition.PartitionTitle = partition.PartitionTitle
                        .Substring(0, partition.PartitionTitle.Length - 2);

                context.CollectedFacts.Add(new QuickFact
                {
                    Title = "Affected " + partition.PartitionTitle.ToLower().Pluralize(),
                    Description = "Known total was either specified in the administration pages, or aggregated by coderr from all application incidents.",
                    Value = $"{partition.ValueCount} <em class=\"text-muted\">({partition.ValueCount*100/partition.TotalCount}%)</em>",
                    Url =
                        $"#/application/{context.ApplicationId}/incident/{context.IncidentId}/partitions/{partition.Id}"
                });
            }
        }
    }
}