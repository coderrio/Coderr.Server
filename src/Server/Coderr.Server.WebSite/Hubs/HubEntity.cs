using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coderr.Server.WebSite.Hubs
{
    public class HubEntity
    {
        public string Namespace { get; set; }
        public string TypeName { get; set; }
        public bool IsEvent { get; set; }
        
    }
}
