using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Events;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Modules.History;
using DotNetCqs;

namespace Coderr.Server.App.Modules.History.Events
{
    public class IncidentAssignedHandler : IMessageHandler<IncidentAssigned>
    {
        private readonly IHistoryRepository _repository;

        public IncidentAssignedHandler(IHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, IncidentAssigned message)
        {
            //var entries = await _repository.GetByIncidentId(message.IncidentId);

            //// Include the version to make it easier to fetch information
            //// that should be shown (to give the user a hint on version difference between created and assigned states)
            //var version = entries.Where(x => x.ApplicationVersion != null).Select(x => x.ApplicationVersion)
            //    .LastOrDefault();

            var entry = new HistoryEntry(message.IncidentId, message.AssignedById, IncidentState.Active);
            await _repository.CreateAsync(entry);
        }
    }
}git