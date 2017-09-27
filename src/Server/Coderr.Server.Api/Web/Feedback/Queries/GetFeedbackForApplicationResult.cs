using System.Collections.Generic;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Result for <see cref="GetFeedbackForApplicationPage" />
    /// </summary>
    public class GetFeedbackForApplicationPageResult
    {
        /// <summary>
        ///     All emails (included in the first page)
        /// </summary>
        //TODO: crappy solution
        public List<string> Emails { get; set; }

        /// <summary>
        ///     items on this page
        /// </summary>
        public GetFeedbackForApplicationPageResultItem[] Items { get; set; }

        /// <summary>
        ///     Total number of items
        /// </summary>
        public int TotalCount { get; set; }
    }
}