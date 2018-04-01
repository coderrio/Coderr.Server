using System.Collections.Generic;
using System.Text;
using DotNetCqs;

namespace Coderr.Server.Api.Modules.History.Queries
{
    [Message]
    public class GetIncidentsForStates : Query<GetIncidentsForStatesResult>
    {
        public string ApplicationVersion { get; set; }
        public int ApplicationId { get; set; }

        public bool IsClosed { get; set; }
        public bool IsNew { get; set; }

        public bool IsReopened { get; set; }
    }
}
