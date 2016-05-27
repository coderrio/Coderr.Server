using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Incidents.Commands;
using OneTrueError.Api.Core.Incidents.Events;
using OneTrueError.App.Core.Users;

namespace OneTrueError.App.Core.Incidents.Commands
{
    /// <summary>
    ///     Handler for <see cref="IgnoreIncident" />.
    /// </summary>
    [Component]
    public class IgnoreIncidentHandler : ICommandHandler<IgnoreIncident>
    {
        private readonly IEventBus _eventBus;
        private readonly IIncidentRepository _incidentRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="IgnoreIncidentHandler" />.
        /// </summary>
        /// <param name="incidentRepository">to load and update info about the incident that is being ignored</param>
        /// <param name="userRepository">to get info about the user that ignores the incident</param>
        /// <param name="eventBus">to publish ignore event</param>
        /// <exception cref="ArgumentNullException"></exception>
        public IgnoreIncidentHandler(IIncidentRepository incidentRepository, IUserRepository userRepository,
            IEventBus eventBus)
        {
            if (incidentRepository == null) throw new ArgumentNullException("incidentRepository");
            if (userRepository == null) throw new ArgumentNullException("userRepository");
            if (eventBus == null) throw new ArgumentNullException("eventBus");

            _incidentRepository = incidentRepository;
            _userRepository = userRepository;
            _eventBus = eventBus;
        }

        /// <summary>Execute a command asynchronously.</summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>Task which will be completed once the command has been executed.</returns>
        public async Task ExecuteAsync(IgnoreIncident command)
        {
            var user = await _userRepository.GetUserAsync(command.UserId);
            var incident = await _incidentRepository.GetAsync(command.IncidentId);
            incident.IgnoreFutureReports(user.UserName);
            await _incidentRepository.UpdateAsync(incident);

            await _eventBus.PublishAsync(new IncidentIgnored(command.IncidentId, user.AccountId, user.UserName));
        }
    }
}