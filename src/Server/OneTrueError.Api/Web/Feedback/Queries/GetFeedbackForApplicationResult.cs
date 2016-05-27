using System.Collections.Generic;

namespace OneTrueError.Api.Web.Feedback.Queries
{
    /// <summary>
    /// Result for <see cref="GetFeedbackForApplicationPage"/>
    /// </summary>
    public class GetFeedbackForApplicationPageResult
    {
        /// <summary>
        /// Total number of items
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// items on this page
        /// </summary>
        public GetFeedbackForApplicationPageResultItem[] Items { get; set; }

        /// <summary>
        /// All emails (included in the first page)
        /// </summary>
        //TODO: crappy solution
        public List<string> Emails { get; set; }
    }
}