using System.Collections.Generic;

namespace Coderr.Server.Api.Core.Settings.Commands
{
    [Command]
    public class SaveAccountSettings
    {
        public int AccountId { get; set; }
        public IDictionary<string, string> Settings { get; set; }
    }
}