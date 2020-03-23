using System;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.UserNotifications.Handlers.Tasks;
using DotNetCqs;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using log4net;

namespace Coderr.Server.ReportAnalyzer.UserNotifications.Handlers
{
    /// <summary>
    ///     Responsible of sending notifications when a new report have been analyzed.
    /// </summary>
    public class CheckForNotificationsToSend : IMessageHandler<ReportAddedToIncident>
    {
        private readonly IUserNotificationsRepository _notificationsRepository;
        private readonly IUserRepository _userRepository;
        private readonly BaseConfiguration _configuration;
        private readonly ILog _log = LogManager.GetLogger(typeof(CheckForNotificationsToSend));
        private INotificationService _notificationService;

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForNotificationsToSend" />.
        /// </summary>
        /// <param name="notificationsRepository">To load notification configuration</param>
        /// <param name="userRepository">To load user info</param>
        public CheckForNotificationsToSend(IUserNotificationsRepository notificationsRepository,
            IUserRepository userRepository, IConfiguration<BaseConfiguration> configuration, INotificationService notificationService)
        {
            _notificationsRepository = notificationsRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
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
            if (e == null) throw new ArgumentNullException(nameof(e));

            _log.Info("ReportId: " + e.Report.Id);

            var settings = await _notificationsRepository.GetAllAsync(e.Incident.ApplicationId);
            foreach (var setting in settings)
            {
                if (setting.NewIncident != NotificationState.Disabled && e.IsNewIncident == true)
                {
                    if (string.IsNullOrEmpty(e.EnvironmentName)
                    || e.EnvironmentName.Equals("production", StringComparison.OrdinalIgnoreCase)
                        || e.EnvironmentName.Equals("prod", StringComparison.OrdinalIgnoreCase))
                    {
                        await CreateNotification(context, e, setting.AccountId, setting.NewIncident);
                    }
                    else
                    {
                        _log.Debug("Error was new, but not for the production environment: " + e.EnvironmentName);
                    }
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
            if (state == NotificationState.BrowserNotification)
            {
                var notification = new Notification($"Application: {e.Incident.ApplicationName}\r\n{e.Incident.Name}");
                notification.Actions.Add(new NotificationAction()
                {
                    Title = "Assign to me",
                    Action = "AssignToMe"
                });
                notification.Actions.Add(new NotificationAction()
                {
                    Title = "View",
                    Action = "View"
                });
                notification.Icon = "/favicon-32x32.png";
                notification.Timestamp = e.Report.CreatedAtUtc;
                notification.Title = e.IsNewIncident == true
                    ? "New incident"
                    : "Re-opened incident";
                notification.Data = new
                {
                    applicationId = e.Incident.ApplicationId,
                    incidentId = e.Incident.Id
                };
                await _notificationService.SendBrowserNotification(accountId, notification);
            }
            else if (state == NotificationState.Email)
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