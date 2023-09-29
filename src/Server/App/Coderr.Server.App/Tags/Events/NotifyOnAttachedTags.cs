using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Api.Modules.Tagging.Events;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.Abstractions.Notifications.Commands;
using DotNetCqs;

namespace Coderr.Server.App.Tags.Events
{
    public class NotifyOnAttachedTags : IMessageHandler<TagAttachedToIncident>
    {
        private readonly BaseConfiguration _baseConfiguration;
        private readonly IUserNotificationsRepository _repository;
        private readonly IApplicationRepository _applicationRepository;
        private IMessageContext _context;
        private readonly IIncidentRepository _incidentRepository;

        public NotifyOnAttachedTags(IUserNotificationsRepository repository, IConfiguration<BaseConfiguration> baseConfiguration,
            IApplicationRepository applicationRepository, IIncidentRepository incidentRepository)
        {
            _repository = repository;
            _baseConfiguration = baseConfiguration.Value;
            _applicationRepository = applicationRepository;
            _incidentRepository = incidentRepository;
        }


        public async Task HandleAsync(IMessageContext context, TagAttachedToIncident message)
        {
            _context = context;
            var notificationSettings = (await _repository.GetAllAsync(message.ApplicationId)).ToList();
            if (notificationSettings.All(x =>
                x.CriticalIncident == NotificationState.Disabled && x.ImportantIncident == NotificationState.Disabled))
                return;

            var isImportant = message.Tags.Any(x => x == "important");
            var isCritical = message.Tags.Any(x => x == "critical");
            if (!isImportant && !isCritical) return;

            var app = await _applicationRepository.GetByIdAsync(message.ApplicationId);
            foreach (var setting in notificationSettings)
            {
                if (setting.CriticalIncident == NotificationState.Disabled &&
                    setting.ImportantIncident == NotificationState.Disabled)
                    continue;

                var appName = app.Name;

                if (setting.CriticalIncident != NotificationState.Disabled && isCritical)
                {
                    var state = setting.CriticalIncident;
                    var tagStatement = "a critical";
                    await SendNotification(context, message, state, setting, tagStatement, appName);
                }
                else if (setting.ImportantIncident != NotificationState.Disabled && isImportant)
                {
                    var state = setting.ImportantIncident;
                    var tagStatement = "an important";
                    await SendNotification(context, message, state, setting, tagStatement, appName);
                }
            }
        }

        private async Task<SendBrowserNotification> BuildBrowserNotification(TagAttachedToIncident e, string appName,
            string tagStatement, UserNotificationSettings setting)
        {
            var incident = await _incidentRepository.GetAsync(e.IncidentId);
            var notification =
                new SendBrowserNotification(setting.AccountId)
                {
                    Body = $"Application: {appName}\r\nTitle: {incident.Description}",
                    Title = $"Coderr have received {tagStatement} incident.",
                    Actions = new List<SendBrowserNotificationAction>
                    {
                        new SendBrowserNotificationAction {Title = "View", Action = "viewIncident"}
                    },
                    UserData = new
                    {
                        applicationId = e.ApplicationId,
                        incidentId = e.IncidentId,
                        accountId = setting.AccountId,
                        viewIncidentUrl = $"{_baseConfiguration.BaseUrl}/go/incident/{e.IncidentId}"
                    },
                    Timestamp = incident.CreatedAtUtc
                };
            return notification;
        }

        private async Task<EmailMessage> BuildEmail(TagAttachedToIncident e, UserNotificationSettings setting,
            string tagStatement, string appName)
        {
            var incident = await _incidentRepository.GetAsync(e.IncidentId);
            var msg = new EmailMessage(setting.AccountId.ToString())
            {
                Subject = $"Received {tagStatement} incident for application {appName}",
                HtmlBody =
                    $"We've received a new incident for application {appName}\r\n" +
                    $"{_baseConfiguration.BaseUrl}/go/incident/{e.IncidentId}\r\n" +
                    "\r\n" +
                    $"Name: {incident.Description}\r\n" +
                    $"Type: {incident.FullName}\r\n"
            };
            return msg;
        }

        private async Task SendNotification(IMessageContext context, TagAttachedToIncident message,
            NotificationState state,
            UserNotificationSettings setting, string tagStatement, string appName)
        {
            if (state == NotificationState.Email)
            {
                var msg = await BuildEmail(message, setting, tagStatement, appName);
                var cmd = new SendEmail(msg);
                await context.SendAsync(cmd);
            }
            else
            {
                var notification = await BuildBrowserNotification(message, appName, tagStatement, setting);
                try
                {
                    await _context.SendAsync(notification);
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new {setting, notification});
                }
            }
        }
    }
}