using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Events;
using codeRR.Server.App.Core.Notifications.Tasks;
using codeRR.Server.App.Core.Users;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Notifications.EventHandlers
{
    /// <summary>
    ///     Responsible of sending notifications when a new report have been analyzed.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class CheckForNotificationsToSend :
        IMessageHandler<ReportAddedToIncident>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IUserRepository _userRepository;
        private ConfigurationStore _configStore;

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForNotificationsToSend" />.
        /// </summary>
        /// <param name="notificationsRepository">To load notification configuration</param>
        /// <param name="userRepository">To load user info</param>
        public CheckForNotificationsToSend(INotificationsRepository notificationsRepository,
            IUserRepository userRepository, ConfigurationStore configStore)
        {
            _notificationsRepository = notificationsRepository;
            _userRepository = userRepository;
            _configStore = configStore;
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
                var email = new SendIncidentEmail(_configStore);
                await email.SendAsync(context, accountId.ToString(), e.Incident, e.Report);
            }
            else
            {
                var handler = new SendIncidentSms(_userRepository, _configStore);
                await handler.SendAsync(accountId, e.Incident, e.Report);
            }
        }
    }
}