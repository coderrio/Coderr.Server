using DotNetCqs;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Get given feedback for all applications.
    /// </summary>
    [Message]
    public class GetFeedbackForDashboardPage : Query<GetFeedbackForDashboardPageResult>
    {
    }
}