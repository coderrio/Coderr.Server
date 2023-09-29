using System.Threading.Tasks;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Core.Incidents.Events;
using Coderr.Server.Domain.Modules.History;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;

namespace Coderr.Server.App.Modules.History.Events
{
    internal class IncidentCreatedHandler : IMessageHandler<IncidentCreated>
    {
        private readonly IHistoryRepository _historyRepository;

        public IncidentCreatedHandler(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task HandleAsync(IMessageContext context, IncidentCreated message)
        {
            var entry = new HistoryEntry(message.IncidentId, null, IncidentState.New)
            {
                ApplicationVersion = message.ApplicationVersion
            };
            await _historyRepository.CreateAsync(entry);
        }
    }
}