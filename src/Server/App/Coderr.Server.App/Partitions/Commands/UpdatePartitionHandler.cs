using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Commands;
using Coderr.Server.Domain.Modules.Partitions;
using DotNetCqs;

namespace Coderr.Server.App.Partitions.Commands
{
    public class UpdatePartitionHandler : IMessageHandler<UpdatePartition>
    {
        private readonly IPartitionRepository _repository;

        public UpdatePartitionHandler(IPartitionRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, UpdatePartition message)
        {
            var partition = await _repository.GetByIdAsync(message.Id);
            partition.Name = message.Name;
            partition.NumberOfItems = message.NumberOfItems;
            partition.Weight = message.Weight;

            if (message.CriticalThreshold > 0)
                partition.CriticalThreshold = message.CriticalThreshold;
            if (message.ImportantThreshold > 0)
                partition.ImportantThreshold = message.ImportantThreshold;

            await _repository.UpdateAsync(partition);
        }
    }
}