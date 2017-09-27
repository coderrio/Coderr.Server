using codeRR.Server.Api.Core.Users.Queries;

namespace codeRR.Server.Api.Core.Users
{
    /// <summary>
    ///     Notification settings for <see cref="GetUserSettingsResult" />.
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        ///     How to notify when a new incident is created (received an unique exception)
        /// </summary>
        public NotificationState NotifyOnNewIncidents { get; set; }

        /// <summary>
        ///     How to notify when a new report is created (receive an exception)
        /// </summary>
        public NotificationState NotifyOnNewReport { get; set; }

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
    }
}