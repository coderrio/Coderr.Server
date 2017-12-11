using System.Collections.Generic;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.PluginApi.Incidents
{
    public interface ISolutionProvider
    {
        Task SuggestSolutionAsync(int incidentId, ICollection<SuggestedIncidentSolution> suggestedSolutions);
    }
}