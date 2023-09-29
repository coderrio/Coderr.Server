using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Api.WorkItems.Commands;
using DotNetCqs;

namespace Coderr.Server.App.WorkItems.Commands
{
    class CreateWorkItemHandler : IMessageHandler<CreateWorkItem>
    {
        private IWorkItemServiceProvider _provider;

        public CreateWorkItemHandler(IWorkItemServiceProvider provider)
        {
            _provider = provider;
        }


        public async Task HandleAsync(IMessageContext context, CreateWorkItem message)
        {
            await _provider.CreateAsync(message.ApplicationId, message.IncidentId);
        }
    }
}
