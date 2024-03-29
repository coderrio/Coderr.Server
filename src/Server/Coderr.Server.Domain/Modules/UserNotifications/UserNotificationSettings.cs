﻿using System;

namespace Coderr.Server.Domain.Modules.UserNotifications
{
    /// <summary>
    ///     Account settings for notifications
    /// </summary>
    public class UserNotificationSettings
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UserNotificationSettings" />.
        /// </summary>
        /// <param name="accountId">Account id</param>
        /// <param name="applicationId">Application (0 = general setting)</param>
        /// <exception cref="ArgumentOutOfRangeException">accountId</exception>
        public UserNotificationSettings(int accountId, int applicationId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            AccountId = accountId;
            ApplicationId = applicationId;
            if (applicationId != 0)
                return;

            ApplicationSpike = NotificationState.Disabled;
            NewIncident = NotificationState.Disabled;
            ReopenedIncident = NotificationState.Disabled;
            UserFeedback = NotificationState.Disabled;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected UserNotificationSettings()
        {
        }

        /// <summary>
        ///     Account id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        ///     Application id (0 = general configuration)
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Notify when a report spike is detected for an application
        /// </summary>
        public NotificationState ApplicationSpike { get; set; }

        /// <summary>
        ///     How to notify when a new incident is created (received a new unique exception).
        /// </summary>
        public NotificationState NewIncident { get; set; }

        /// <summary>
        ///     How to notify when an incident is updated to critical.
        /// </summary>
        public NotificationState CriticalIncident { get; set; }

        /// <summary>
        ///     How to notify when an incident is updated to important.
        /// </summary>
        public NotificationState ImportantIncident { get; set; }

        /// <summary>
        ///     Notify when we received a report for an incident that has been closed
        /// </summary>
        public NotificationState ReopenedIncident { get; set; }

        /// <summary>
        ///     Notify when a user have written an error description
        /// </summary>
        public NotificationState UserFeedback { get; set; }

        /// <summary>
        ///     Send a weekly summary for all applications that the user is a member of.
        /// </summary>
        public NotificationState WeeklySummary { get; set; }
    }
}