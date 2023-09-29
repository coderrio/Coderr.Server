using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Queries;
using Coderr.Server.Domain.Modules.Partitions;
using DotNetCqs;

namespace Coderr.Server.App.Partitions.Queries
{
    internal class GetPartitionsHandler : IQueryHandler<GetPartitions, GetPartitionsResult>
    {
        private readonly IPartitionRepository _repository;

        public GetPartitionsHandler(IPartitionRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPartitionsResult> HandleAsync(IMessageContext context, GetPartitions query)
        {
            var partitions = await _repository.GetDefinitions(query.ApplicationId);
            var items = partitions
                .Select(x => new GetPartitionsResultItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartitionKey = x.PartitionKey,
                    Weight = x.Weight
                })
                .OrderBy(x => x.Name)
                .ToArray();
            return new GetPartitionsResult
            {
                Items = items
            };
        }
    }
}