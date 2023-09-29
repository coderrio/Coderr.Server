using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coderr.Server.App.Modules.Mine
{
    /// <summary>
    /// Will gather all recommendations and sort them by the number of points.
    /// </summary>
    public interface IRecommendationService
    {
        Task<List<RecommendedIncident>> GetRecommendations(int accountId, int? applicationId = null);
    }
}
