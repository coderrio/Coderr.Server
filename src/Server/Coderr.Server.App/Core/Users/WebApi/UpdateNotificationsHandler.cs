﻿using System;
using System.Threading.Tasks;
using Coderr.Server.Api;
using Coderr.Server.Api.Core.Users.Commands;
using Coderr.Server.App.Core.Notifications;
using Coderr.Server.Domain.Modules.UserNotifications;
using DotNetCqs;


namespace Coderr.Server.App.Core.Users.WebApi
{
    /// <summary>
    ///     Handler for <see cref="UpdateNotifications" />.
    /// </summary>
    public class UpdateNotificationsHandler : IMessageHandler<UpdateNotifications>
    {
        private readonly INotificationsRepository _notificationsRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdateNotificationsHandler" />.
        /// </summary>
        /// <param name="notificationsRepository">notifications repository</param>
        /// <exception cref="ArgumentNullException">notificationsRepository</exception>
        public UpdateNotificationsHandler(INotificationsRepository notificationsRepository)
        {
            if (notificationsRepository == null) throw new ArgumentNullException("notificationsRepository");
            _notificationsRepository = notificationsRepository;
        }

        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, UpdateNotifications command)
        {
            var settings = await _notificationsRepository.TryGetAsync(command.UserId, command.ApplicationId);
            if (settings == null)
            {
                settings = new UserNotificationSettings(command.UserId, command.ApplicationId);
                await _notificationsRepository.CreateAsync(settings);
            }

            settings.ApplicationSpike = command.NotifyOnPeaks.ConvertEnum<NotificationState>();
            settings.NewIncident = command.NotifyOnNewIncidents.ConvertEnum<NotificationState>();
            settings.CriticalIncident = command.NotifyOnCriticalIncidents.ConvertEnum<NotificationState>();
            settings.ImportantIncident = command.NotifyOnImportantIncidents.ConvertEnum<NotificationState>();
            settings.ReopenedIncident = command.NotifyOnReOpenedIncident.ConvertEnum<NotificationState>();
            settings.UserFeedback = command.NotifyOnUserFeedback.ConvertEnum<NotificationState>();
            await _notificationsRepository.UpdateAsync(settings);
        }
    }
}