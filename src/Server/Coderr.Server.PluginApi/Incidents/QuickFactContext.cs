using System;
using System.Collections.Generic;
using codeRR.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.PluginApi.Incidents
{
    public class QuickFactContext
    {
        public QuickFactContext(int applicationId, int incidentId, ICollection<QuickFact> facts)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            ApplicationId = applicationId;
            IncidentId = incidentId;
            CollectedFacts = facts;
        }
        public int IncidentId { get; private set; }
        public int ApplicationId { get; private set; }
        public ICollection<QuickFact> CollectedFacts { get; private set; }
    }
}