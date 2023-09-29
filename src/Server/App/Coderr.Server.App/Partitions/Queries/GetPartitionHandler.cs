using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Queries;
using Coderr.Server.Domain.Modules.Partitions;
using DotNetCqs;

namespace Coderr.Server.App.Partitions.Queries
{
    internal class GetPartitionHandler : IQueryHandler<GetPartition, GetPartitionResult>
    {
        private readonly IPartitionRepository _repository;

        public GetPartitionHandler(IPartitionRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPartitionResult> HandleAsync(IMessageContext context, GetPartition query)
        {
            var partition = await _repository.GetByIdAsync(query.Id);

            return new GetPartitionResult
            {
                Id = partition.Id,
                Name = partition.Name,
                NumberOfItems = partition.NumberOfItems,
                PartitionKey = partition.PartitionKey,
                Weight = partition.Weight,
                ImportantThreshold = partition.ImportantThreshold,
                CriticalThreshold = partition.CriticalThreshold
            };
        }
    }
}