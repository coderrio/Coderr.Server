using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Incidents.Commands;
using Coderr.Server.Api.Core.Incidents.Events;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;


namespace Coderr.Server.App.Core.Incidents.Commands
{
    /// <summary>
    ///     Handler for <see cref="AssignIncident" />
    /// </summary>
    public class AssignIncidentHandler : IMessageHandler<AssignIncident>
    {
        private readonly IIncidentRepository _repository;

        public AssignIncidentHandler(IIncidentRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, AssignIncident message)
        {
            var assignedBy = message.AssignedBy;
            if (assignedBy != 0)
                assignedBy = context.Principal.GetAccountId();

            var incident = await _repository.GetAsync(message.IncidentId);
            incident.Assign(message.AssignedTo, message.AssignedAtUtc);
            await _repository.UpdateAsync(incident);

            var evt = new IncidentAssigned(message.IncidentId, assignedBy, message.AssignedTo);
            await context.SendAsync(evt);
        }
    }
}