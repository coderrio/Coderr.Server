using System;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Result item for <see cref="GetIncidentFeedbackResult" />.
    /// </summary>
    public class GetIncidentFeedbackResultItem
    {
        /// <summary>
        ///     Email if user can be contacted.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Error description written by the user that experienced the error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     When the feedback was written
        /// </summary>
        public DateTime WrittenAtUtc { get; set; }
    }
}