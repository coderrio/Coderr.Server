using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Commands;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;
using Griffin.Container;

namespace Coderr.Server.App.Core.Incidents.Commands
{
    /// <summary>
    /// Uses the incident repository and the domain entity to apply the change.
    /// </summary>
    public class ReOpenIncidentHandler : IMessageHandler<ReOpenIncident>
    {
        private readonly IIncidentRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="CloseIncidentHandler" />.
        /// </summary>
        /// <param name="repository">To be able to load and update incident</param>
        public ReOpenIncidentHandler(IIncidentRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }


        /// <inheritdoc/>
        public async Task HandleAsync(IMessageContext context, ReOpenIncident command)
        {
            var incident = await _repository.GetAsync(command.IncidentId);
            incident.Reopen();
            await _repository.UpdateAsync(incident);

            var evt = new IncidentReOpened(incident.ApplicationId, incident.Id, DateTime.UtcNow);
            await context.SendAsync(evt);
        }
    }
}
