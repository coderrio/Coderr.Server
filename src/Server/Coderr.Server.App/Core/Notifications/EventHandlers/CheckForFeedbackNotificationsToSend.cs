using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Queries;
using codeRR.Server.Api.Core.Feedback.Events;
using codeRR.Server.Api.Core.Incidents.Queries;
using codeRR.Server.Api.Core.Messaging;
using codeRR.Server.Api.Core.Messaging.Commands;
using codeRR.Server.App.Configuration;
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
    public class CheckForFeedbackNotificationsToSend :
        IMessageHandler<FeedbackAttachedToIncident>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private ConfigurationStore _configStore;

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForNotificationsToSend" />.
        /// </summary>
        /// <param name="notificationsRepository">To load notification configuration</param>
        public CheckForFeedbackNotificationsToSend(INotificationsRepository notificationsRepository, ConfigurationStore configStore)
        {
            _notificationsRepository = notificationsRepository;
            _configStore = configStore;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(IMessageContext context, FeedbackAttachedToIncident e)
        {
            var settings = await _notificationsRepository.GetAllAsync(-1);
            var incident = await context.QueryAsync(new GetIncident(e.IncidentId));
            foreach (var setting in settings)
            {
                if (setting.UserFeedback == NotificationState.Disabled)
                    continue;

                var notificationEmail = await context.QueryAsync(new GetAccountEmailById(setting.AccountId));
                var config = _configStore.Load<BaseConfiguration>();

                var shortName = incident.Description.Length > 40
                    ? incident.Description.Substring(0, 40) + "..."
                    : incident.Description;

                if (string.IsNullOrEmpty(e.UserEmailAddress))
                    e.UserEmailAddress = "unknown";

                var incidentUrl = string.Format("{0}/#/application/{1}/incident/{2}",
                    config.BaseUrl.ToString().TrimEnd('/'),
                    incident.ApplicationId,
                    incident.Id);

                //TODO: Add more information
                var msg = new EmailMessage(notificationEmail);
                msg.Subject = "New feedback: " + shortName;
                msg.TextBody = string.Format(@"Incident: {0}
Feedback: {0}/feedback
From: {1}

{2}
", incidentUrl, e.UserEmailAddress, e.Message);


                var emailCmd = new SendEmail(msg);
                await context.SendAsync(emailCmd);
            }
        }
    }
}