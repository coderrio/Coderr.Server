namespace Coderr.Server.Api.Core.Settings.Commands
{
    [Command]
    public class SaveAccountSetting
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

    }
}