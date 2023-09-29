using DotNetCqs;

namespace Coderr.Server.Api.Core.Settings.Queries
{
    [Message]
    public class GetAccountSetting : Query<GetAccountSettingResult>
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
    }
}