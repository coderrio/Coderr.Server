using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Incidents.Commands;
using OneTrueError.Api.Core.Incidents.Events;

namespace OneTrueError.App.Core.Incidents.Commands
{
    /// <summary>
    /// Uses the incident repository and the domain entity to apply the change.
    /// </summary>
    [Component]
    public class ReOpenIncidentHandler : ICommandHandler<ReOpenIncident>
    {
        private readonly IIncidentRepository _repository;
        private readonly IEventBus _eventBus;

        /// <summary>
        ///     Creates a new instance of <see cref="CloseIncidentHandler" />.
        /// </summary>
        /// <param name="repository">To be able to load and update incident</param>
        /// <param name="eventBus">Used to publish <see cref="IncidentReOpened"/></param>
        public ReOpenIncidentHandler(IIncidentRepository repository, IEventBus eventBus)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _eventBus = eventBus;
        }


        /// <inheritdoc/>
        public async Task ExecuteAsync(ReOpenIncident command)
        {
            var incident = await _repository.GetAsync(command.IncidentId);
            incident.Reopen();
            await _repository.UpdateAsync(incident);

            var evt = new IncidentReOpened(incident.ApplicationId, incident.Id, DateTime.UtcNow);
            await _eventBus.PublishAsync(evt);
        }
    }
}
