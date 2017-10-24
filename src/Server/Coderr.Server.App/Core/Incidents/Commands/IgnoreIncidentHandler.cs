using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Commands;
using codeRR.Server.Api.Core.Incidents.Events;
using codeRR.Server.App.Core.Users;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Incidents.Commands
{
    /// <summary>
    ///     Handler for <see cref="IgnoreIncident" />.
    /// </summary>
    [Component]
    public class IgnoreIncidentHandler : IMessageHandler<IgnoreIncident>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="IgnoreIncidentHandler" />.
        /// </summary>
        /// <param name="incidentRepository">to load and update info about the incident that is being ignored</param>
        /// <param name="userRepository">to get info about the user that ignores the incident</param>
        /// <exception cref="ArgumentNullException"></exception>
        public IgnoreIncidentHandler(IIncidentRepository incidentRepository, IUserRepository userRepository)
        {
            if (incidentRepository == null) throw new ArgumentNullException("incidentRepository");
            if (userRepository == null) throw new ArgumentNullException("userRepository");

            _incidentRepository = incidentRepository;
            _userRepository = userRepository;
        }

        /// <summary>Execute a command asynchronously.</summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>Task which will be completed once the command has been executed.</returns>
        public async Task HandleAsync(IMessageContext context, IgnoreIncident command)
        {
            var user = await _userRepository.GetUserAsync(command.UserId);
            var incident = await _incidentRepository.GetAsync(command.IncidentId);
            incident.IgnoreFutureReports(user.UserName);
            await _incidentRepository.UpdateAsync(incident);

            await context.SendAsync(new IncidentIgnored(command.IncidentId, user.AccountId, user.UserName));
        }
    }
}