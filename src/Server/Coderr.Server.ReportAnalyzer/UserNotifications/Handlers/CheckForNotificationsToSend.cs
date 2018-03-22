using System;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.UserNotifications.Handlers.Tasks;
using DotNetCqs;
using Griffin.Container;

namespace Coderr.Server.ReportAnalyzer.UserNotifications.Handlers
{
    /// <summary>
    ///     Responsible of sending notifications when a new report have been analyzed.
    /// </summary>
    [ContainerService]
    public class CheckForNotificationsToSend :
        IMessageHandler<ReportAddedToIncident>
    {
        private readonly IUserNotificationsRepository _notificationsRepository;
        private readonly IUserRepository _userRepository;
        private readonly BaseConfiguration _configuration;

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForNotificationsToSend" />.
        /// </summary>
        /// <param name="notificationsRepository">To load notification configuration</param>
        /// <param name="userRepository">To load user info</param>
        public CheckForNotificationsToSend(IUserNotificationsRepository notificationsRepository,
            IUserRepository userRepository, IConfiguration<BaseConfiguration> configuration)
        {
            _notificationsRepository = notificationsRepository;
            _userRepository = userRepository;
            _configuration = configuration.Value;
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            if (e == null) throw new ArgumentNullException("e");

            var settings = await _notificationsRepository.GetAllAsync(e.Incident.ApplicationId);
            foreach (var setting in settings)
            {
                if (setting.NewIncident != NotificationState.Disabled && e.Incident.ReportCount == 1)
                {
                    await CreateNotification(context, e, setting.AccountId, setting.NewIncident);
                }
                else if (setting.NewReport != NotificationState.Disabled)
                {
                    await CreateNotification(context, e, setting.AccountId, setting.NewReport);
                }
                else if (setting.ReopenedIncident != NotificationState.Disabled && e.IsReOpened)
                {
                    await CreateNotification(context, e, setting.AccountId, setting.ReopenedIncident);
                }
            }
        }

        private async Task CreateNotification(IMessageContext context, ReportAddedToIncident e, int accountId,
            NotificationState state)
        {
            if (state == NotificationState.Email)
            {
                var email = new SendIncidentEmail(_configuration);
                await email.SendAsync(context, accountId.ToString(), e.Incident, e.Report);
            }
            else
            {
                var handler = new SendIncidentSms(_userRepository, _configuration);
                await handler.SendAsync(accountId, e.Incident, e.Report);
            }
        }
    }
}