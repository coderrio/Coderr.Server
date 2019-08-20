using System;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Incidents.Commands;
using Coderr.Server.Api.Core.Incidents.Events;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;

namespace Coderr.Server.App.Core.Incidents.Commands
{
    /// <summary>
    ///     Handler for <see cref="DeleteIncident" />
    /// </summary>
    public class DeleteIncidentHandler : IMessageHandler<DeleteIncident>
    {
        private readonly IIncidentRepository _repository;

        public DeleteIncidentHandler(IIncidentRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, DeleteIncident message)
        {
            var incident = await _repository.GetAsync(message.IncidentId);
            await _repository.Delete(message.IncidentId);

            var evt = new IncidentDeleted(message.IncidentId, message.UserId ?? context.Principal.GetAccountId(),
                message.DeletedAtUtc ?? DateTime.UtcNow);
            await context.SendAsync(evt);
        }
    }
}