using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.App.IncidentInsights.Subscribers;
using Coderr.Server.Domain.Modules.Statistics;

namespace Coderr.Server.App.IncidentInsights
{
    public interface IIncidentProgressRepository
    {
        Task<List<IncidentProgressTracker>> FindAssigned(int[] applicationIds, DateTime @from, DateTime to);
        Task<List<IncidentProgressTracker>> FindCreated(int[] applicationIds, DateTime @from, DateTime to);
        Task<List<IncidentProgressTracker>> FindClosed(int[] applicationIds, DateTime @from, DateTime to);

        IDictionary<int, int>  CountAll(int[] applicationIds, DateTime from, DateTime to);
    }
}
