using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Events;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Modules.History;
using DotNetCqs;

namespace Coderr.Server.App.Modules.History.Events
{
    internal class IncidentClosedHandler : IMessageHandler<IncidentClosed>
    {
        private readonly IHistoryRepository _repository;

        public IncidentClosedHandler(IHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, IncidentClosed message)
        {
            var entry = new HistoryEntry(message.IncidentId, message.ClosedById, IncidentState.Closed)
            {
                ApplicationVersion = message.ApplicationVersion
            };
            await _repository.CreateAsync(entry);
        }
    }
}