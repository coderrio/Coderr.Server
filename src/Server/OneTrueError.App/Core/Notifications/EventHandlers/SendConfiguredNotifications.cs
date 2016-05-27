using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Incidents.Events;
using OneTrueError.App.Core.Notifications.Tasks;
using OneTrueError.App.Core.Users;

namespace OneTrueError.App.Core.Notifications.EventHandlers
{
    /// <summary>
    ///     Responsible of sending notifcations when a new report have been analyzed.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class SendConfiguredNotifications :
        IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private readonly ICommandBus _commandBus;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="SendConfiguredNotifications" />.
        /// </summary>
        /// <param name="notificationsRepository">To load notification configuration</param>
        /// <param name="commandBus">To send emails</param>
        /// <param name="userRepository">To load user info</param>
        public SendConfiguredNotifications(INotificationsRepository notificationsRepository, ICommandBus commandBus,
            IUserRepository userRepository)
        {
            _notificationsRepository = notificationsRepository;
            _commandBus = commandBus;
            _userRepository = userRepository;
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(ReportAddedToIncident e)
        {
            if (e == null) throw new ArgumentNullException("e");

            var settings = await _notificationsRepository.GetAllAsync(e.Incident.ApplicationId);
            foreach (var setting in settings)
            {
                if (setting.NewIncident != NotificationState.Disabled && e.Incident.ReportCount == 1)
                {
                    await CreateNotification(e, setting.AccountId, setting.NewIncident);
                }
                else if (setting.NewReport != NotificationState.Disabled)
                {
                    await CreateNotification(e, setting.AccountId, setting.NewReport);
                }
                else if (setting.ReopenedIncident != NotificationState.Disabled && e.IsReOpened)
                {
                    await CreateNotification(e, setting.AccountId, setting.ReopenedIncident);
                }
            }
        }

        private async Task CreateNotification(ReportAddedToIncident e, int accountId,
            NotificationState state)
        {
            if (state == NotificationState.Email)
            {
                var email = new SendIncidentEmail(_commandBus);
                await email.SendAsync(accountId.ToString(), e.Incident, e.Report);
            }
            else
            {
                var handler = new SendIncidentSms(_userRepository);
                await handler.SendAsync(accountId, e.Incident, e.Report);
            }
        }
    }
}