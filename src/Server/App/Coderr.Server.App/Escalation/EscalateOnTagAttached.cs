using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Escalation.Events;
using Coderr.Server.Api.Modules.Tagging.Events;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;

namespace Coderr.Server.App.Escalation
{
    class EscalateOnTagAttached : IMessageHandler<TagAttachedToIncident>
    {
        private readonly IIncidentRepository _repository;
        private readonly IMessageBus _bus;

        public EscalateOnTagAttached(IIncidentRepository repository, IMessageBus bus)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task HandleAsync(IMessageContext context, TagAttachedToIncident message)
        {
            if (message.Tags.Any(x => x.Equals("critical", StringComparison.OrdinalIgnoreCase)))
            {
                await AddIncident(message.IncidentId, message.ApplicationId, EscalationState.Critical);
            }
            else if (message.Tags.Any(x => x.Equals("important", StringComparison.OrdinalIgnoreCase)))
            {
                await AddIncident(message.IncidentId, message.ApplicationId, EscalationState.Important);
            }
        }

        private async Task AddIncident(int incidentId, int applicationId, EscalationState state)
        {
            var incident = await _repository.GetAsync(incidentId);
            var currentState = incident.Escalation;
            if (currentState == state)
            {
                return;
            }

            incident.Escalation = state;
            await _repository.UpdateAsync(incident);

            await _bus.SendAsync(new IncidentEscalated(applicationId, incidentId)
            {
                IsCritical = state == EscalationState.Critical, 
                IsImportant = state == EscalationState.Important
            });
        }
    }
}