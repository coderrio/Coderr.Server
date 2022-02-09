using System.Collections.Generic;

namespace Coderr.Server.Api.Core.Settings.Queries
{
    public class GetAccountSettingsResult
    {
        public IDictionary<string, string> Settings { get; set; }
    }
}