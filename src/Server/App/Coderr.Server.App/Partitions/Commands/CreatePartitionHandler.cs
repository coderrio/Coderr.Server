using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Commands;
using Coderr.Server.Domain.Modules.Partitions;
using DotNetCqs;

namespace Coderr.Server.App.Partitions.Commands
{
    public class CreatePartitionHandler : IMessageHandler<CreatePartition>
    {
        private readonly IPartitionRepository _repository;

        public CreatePartitionHandler(IPartitionRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, CreatePartition message)
        {
            var partitionDefinition = new PartitionDefinition(message.ApplicationId, message.Name, message.PartitionKey)
            {
                Weight = message.Weight,
                NumberOfItems = message.NumberOfItems,
            };

            if (message.CriticalThreshold > 0)
                partitionDefinition.CriticalThreshold = message.CriticalThreshold;
            if (message.ImportantThreshold > 0)
                partitionDefinition.ImportantThreshold = message.ImportantThreshold;

            await _repository.CreateAsync(partitionDefinition);
        }
    }
}