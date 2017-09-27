using System.Collections.Generic;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Result for <see cref="GetFeedbackForDashboardPage" />
    /// </summary>
    public class GetFeedbackForDashboardPageResult
    {
        /// <summary>
        ///     Emails to all users that are waiting on status updates.
        /// </summary>
        public List<string> Emails { get; set; }

        /// <summary>
        ///     Items on the requested page.
        /// </summary>
        public GetFeedbackForDashboardPageResultItem[] Items { get; set; }

        /// <summary>
        ///     Total number of feedback entries
        /// </summary>
        public int TotalCount { get; set; }
    }
}