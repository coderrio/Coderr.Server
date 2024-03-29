﻿namespace Coderr.Server.Api.Core.Users.Commands
{
    /// <summary>
    ///     Update user notifications
    /// </summary>
    [Message]
    public class UpdateNotifications
    {
        /// <summary>
        ///     Application that the settings is for (0 = general settings)
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     How to notify when a new incident is created (received an unique exception)
        /// </summary>
        public NotificationState NotifyOnNewIncidents { get; set; }

        /// <summary>
        ///     How to notify when an incident is updated to critical.
        /// </summary>
        public NotificationState NotifyOnCriticalIncidents { get; set; }

        /// <summary>
        ///     How to notify when an incident is updated to important.
        /// </summary>
        public NotificationState NotifyOnImportantIncidents { get; set; }

        /// <summary>
        ///     How to notify user when a peak is detected
        /// </summary>
        public NotificationState NotifyOnPeaks { get; set; }

        /// <summary>
        ///     How to notify when we receive a new report on a closed incident.
        /// </summary>
        public NotificationState NotifyOnReOpenedIncident { get; set; }

        /// <summary>
        ///     How to notify when an user have written an error description
        /// </summary>
        public NotificationState NotifyOnUserFeedback { get; set; }

        
        /// <summary>
        ///     User that configured its settings.
        /// </summary>
        public int UserId { get; set; }
    }
}