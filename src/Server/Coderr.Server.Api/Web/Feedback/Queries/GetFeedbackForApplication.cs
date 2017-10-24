using System;
using DotNetCqs;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Get all feedback that is for a specific application
    /// </summary>
    [Message]
    public class GetFeedbackForApplicationPage : Query<GetFeedbackForApplicationPageResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetFeedbackForApplicationPage" />.
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public GetFeedbackForApplicationPage(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetFeedbackForApplicationPage()
        {
        }

        /// <summary>
        ///     Application id
        /// </summary>
        public int ApplicationId { get; private set; }
    }
}