using DotNetCqs;

namespace Coderr.Server.Api.Insights.Queries
{
    [Message]
    public class GetInsights : Query<GetInsightsResult>
    {
        public int? ApplicationId { get; set; }
    }
}
