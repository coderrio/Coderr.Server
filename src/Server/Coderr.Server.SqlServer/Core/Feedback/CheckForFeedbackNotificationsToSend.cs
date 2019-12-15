using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Accounts.Queries;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.Abstractions.Feedback;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using Coderr.Server.ReportAnalyzer.UserNotifications.Handlers;
using DotNetCqs;

namespace Coderr.Server.SqlServer.Core.Feedback
{
    /// <summary>
    ///     Responsible of sending notifications when a new report have been analyzed.
    /// </summary>
    // MUST be here so that it's used from both queues.
    public class CheckForFeedbackNotificationsToSend :
        IMessageHandler<FeedbackAttachedToIncident>
    {
        private readonly IUserNotificationsRepository _notificationsRepository;
        private readonly string _baseUrl;
        private readonly IIncidentRepository _incidentRepository;
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForNotificationsToSend" />.
        /// </summary>
        /// <param name="notificationsRepository">To load notification configuration</param>
        public CheckForFeedbackNotificationsToSend(IUserNotificationsRepository notificationsRepository, IConfiguration<BaseConfiguration> baseConfig, IIncidentRepository incidentRepository, INotificationService notificationService)
        {
            _notificationsRepository = notificationsRepository;
            _baseUrl = baseConfig.Value.BaseUrl.ToString().Trim('/');
            _incidentRepository = incidentRepository;
            _notificationService = notificationService;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(IMessageContext context, FeedbackAttachedToIncident e)
        {
            var settings = await _notificationsRepository.GetAllAsync(-1);
            var incident = await _incidentRepository.GetAsync(e.IncidentId);
            foreach (var setting in settings)
            {
                if (setting.UserFeedback == NotificationState.Disabled)
                    continue;

                var notificationEmail = await context.QueryAsync(new GetAccountEmailById(setting.AccountId));

                var shortName = incident.Description.Length > 40
                    ? incident.Description.Substring(0, 40) + "..."
                    : incident.Description;

                if (string.IsNullOrEmpty(e.UserEmailAddress))
                    e.UserEmailAddress = "Unknown";

                var incidentUrl = $"{_baseUrl}/discover/{incident.ApplicationId}/incident/{incident.Id}";

                if (setting.UserFeedback == NotificationState.Email)
                {
                    await SendEmail(context, e, notificationEmail, shortName, incidentUrl);
                }
                else if (setting.UserFeedback == NotificationState.BrowserNotification)
                {
                    var msg = $@"Incident: {shortName}
From: {e.UserEmailAddress}
Application: {e.ApplicationName}
{e.Message}";
                    var notification = new Notification(msg)
                    {
                        Title = "New feedback",
                        Data = new
                        {
                            viewFeedbackUrl = $"{incidentUrl}/feedback",
                            incidentId = e.IncidentId,
                            applicationId = incident.ApplicationId
                        },
                        Timestamp = DateTime.UtcNow,
                        Actions = new List<NotificationAction>
                        {
                            new NotificationAction
                            {
                                Title = "View",
                                Action = "viewFeedback"
                            }
                        }
                    };
                    try
                    {
                        await _notificationService.SendBrowserNotification(setting.AccountId, notification);
                    }
                    catch (Exception ex)
                    {
                        Err.Report(ex, new { notification, setting });
                    }

                }

            }
        }

        private static async Task SendEmail(IMessageContext context, FeedbackAttachedToIncident e, string notificationEmail,
            string shortName, string incidentUrl)
        {
            //TODO: Add more information
            var msg = new EmailMessage(notificationEmail)
            {
                Subject = "New feedback: " + shortName,
                TextBody = $@"Incident: {incidentUrl}
Feedback: {incidentUrl}/feedback
From: {e.UserEmailAddress}

{e.Message}"
            };


            var emailCmd = new SendEmail(msg);
            await context.SendAsync(emailCmd);
        }
    }
}