using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.Abstractions.Incidents
{
    public interface ISolutionProvider
    {
        Task SuggestSolutionAsync(int incidentId, ICollection<SuggestedIncidentSolution> suggestedSolutions);
    }
}