using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Commands;
using codeRR.Server.Api.Core.Incidents.Events;
using codeRR.Server.Infrastructure.Security;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Incidents.Commands
{
    /// <summary>
    ///     Handler for <see cref="AssignIncident" />
    /// </summary>
    [Component]
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
            incident.Assign(message.AssignedTo);
            await _repository.UpdateAsync(incident);

            var evt = new IncidentAssigned(message.IncidentId, assignedBy, message.AssignedTo);
            await context.SendAsync(evt);
        }
    }
}