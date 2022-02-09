using System;
using System.Linq;
using Newtonsoft.Json;

namespace Coderr.Server.WebSite.Hubs
{
    public class HubEvent
    {
        public string TypeName { get; set; }
        public Guid CorrelationId { get; set; }
        public object Body { get; set; }
    }
}
