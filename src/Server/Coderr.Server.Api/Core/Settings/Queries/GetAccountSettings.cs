using DotNetCqs;

namespace Coderr.Server.Api.Core.Settings.Queries
{
    [Message]
    public class GetAccountSettings : Query<GetAccountSettingsResult>
    {
        public int AccountId { get; set; }
    }
}