using System;

namespace Coderr.Server.Api.Core.Incidents.Queries
{
    public class RelatedIncident
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int IncidentId { get; set; }
        public string Title { get; set; }
    }
}