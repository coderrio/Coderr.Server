using System;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Result item for <see cref="GetFeedbackForApplicationPageResult" />.
    /// </summary>
    public class GetFeedbackForApplicationPageResultItem
    {
        /// <summary>
        ///     Email adress to the user (if the user want to get status updates for the incident)
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Incident that the feedback belongs to
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        ///     Incident name (typically first line of the exception message)
        /// </summary>
        public string IncidentName { get; set; }

        /// <summary>
        ///     Error description written by the user that experienced the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     When the user wrote the feedback
        /// </summary>
        public DateTime WrittenAtUtc { get; set; }
    }
}