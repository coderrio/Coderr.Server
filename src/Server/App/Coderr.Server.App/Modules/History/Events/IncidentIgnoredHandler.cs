using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Events;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Modules.History;
using DotNetCqs;

namespace Coderr.Server.App.Modules.History.Events
{
    public class IncidentIgnoredHandler : IMessageHandler<IncidentIgnored>
    {
        private readonly IHistoryRepository _repository;

        public IncidentIgnoredHandler(IHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, IncidentIgnored message)
        {
            var entry = new HistoryEntry(message.IncidentId, message.AccountId, IncidentState.Ignored);
            await _repository.CreateAsync(entry);
        }
    }
}