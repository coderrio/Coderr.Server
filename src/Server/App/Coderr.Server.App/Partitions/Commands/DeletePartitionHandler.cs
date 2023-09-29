using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Commands;
using Coderr.Server.Domain.Modules.Partitions;
using DotNetCqs;

namespace Coderr.Server.App.Partitions.Commands
{
    public class DeletePartitionHandler : IMessageHandler<DeletePartition>
    {
        private readonly IPartitionRepository _repository;

        public DeletePartitionHandler(IPartitionRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, DeletePartition message)
        {
            await _repository.DeleteDefinitionByIdAsync(message.Id);
        }
    }
}