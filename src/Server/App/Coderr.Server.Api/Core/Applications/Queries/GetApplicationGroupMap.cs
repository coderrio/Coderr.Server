using DotNetCqs;

namespace Coderr.Server.Api.Core.Applications.Queries
{
    [Message]
    public class GetApplicationGroupMap : Query<GetApplicationGroupMapResult>
    {
        public int? ApplicationId { get; set; }
    }
}