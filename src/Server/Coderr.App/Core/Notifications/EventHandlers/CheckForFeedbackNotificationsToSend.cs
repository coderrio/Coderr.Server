using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Core.Accounts.Queries;
using codeRR.Api.Core.Feedback.Events;
using codeRR.Api.Core.Incidents.Queries;
using codeRR.Api.Core.Messaging;
using codeRR.Api.Core.Messaging.Commands;
using codeRR.App.Configuration;
using codeRR.Infrastructure.Configuration;

namespace codeRR.App.Core.Notifications.EventHandlers
{
    /// <summary>
    ///     Responsible of sending notifications when a new report have been analyzed.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class CheckForFeedbackNotificationsToSend :
        IApplicationEventSubscriber<FeedbackAttachedToIncident>
    {
        private readonly ICommandBus _commandBus;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IQueryBus _queryBus;

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForNotificationsToSend" />.
        /// </summary>
        /// <param name="notificationsRepository">To load notification configuration</param>
        /// <param name="commandBus">To send emails</param>
        /// <param name="queryBus"></param>
        public CheckForFeedbackNotificationsToSend(INotificationsRepository notificationsRepository,
            ICommandBus commandBus,
            IQueryBus queryBus)
        {
            _notificationsRepository = notificationsRepository;
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(FeedbackAttachedToIncident e)
        {
            var settings = await _notificationsRepository.GetAllAsync(-1);
            var incident = await _queryBus.QueryAsync(new GetIncident(e.IncidentId));
            foreach (var setting in settings)
            {
                if (setting.UserFeedback == NotificationState.Disabled)
                    continue;

                var notificationEmail = await _queryBus.QueryAsync(new GetAccountEmailById(setting.AccountId));
                var config = ConfigurationStore.Instance.Load<BaseConfiguration>();

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
                await _commandBus.ExecuteAsync(emailCmd);
            }
        }
    }
}