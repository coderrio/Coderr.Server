using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.Abstractions.Incidents
{
    /// <summary>
    /// Checks if there is a solution available for the current incident.
    /// </summary>
    public interface ISolutionProvider
    {
        Task SuggestSolutionAsync(SolutionProviderContext context);
    }
}