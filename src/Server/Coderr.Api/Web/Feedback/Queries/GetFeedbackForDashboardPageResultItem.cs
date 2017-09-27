using System;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Result item for <see cref="GetFeedbackForDashboardPageResult" />
    /// </summary>
    public class GetFeedbackForDashboardPageResultItem
    {
        /// <summary>
        ///     Application that the feedback was written for
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Name of the application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     Email adress to the user (if the user want to get status updates for the incident)
        /// </summary>
        public string EmailAddress { get; set; }


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