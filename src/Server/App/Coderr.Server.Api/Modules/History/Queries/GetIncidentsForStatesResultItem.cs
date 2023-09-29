using System;

namespace Coderr.Server.Api.Modules.History.Queries
{
    public class GetIncidentsForStatesResultItem
    {
        public int IncidentId { get; set; }
        public string IncidentName { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public bool IsClosed { get; set; }
        public bool IsNew { get; set; }

        public bool IsReopened { get; set; }
    }
}