using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using log4net;

namespace Coderr.Server.App.Modules.Mine
{
    [ContainerService]
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationProvider[] _providers;
        private ILog _logger = LogManager.GetLogger(typeof(RecommendationService));

        public RecommendationService(IEnumerable<IRecommendationProvider> providers)
        {
            _providers = providers.ToArray();
        }

        public async Task<List<RecommendedIncident>> GetRecommendations(int accountId, int? applicationId = null)
        {
            var items = new List<RecommendedIncident>();
            var context = new RecommendIncidentContext(items) { AccountId = accountId, ApplicationId = applicationId };
            foreach (var provider in _providers)
            {
                try
                {
                    await provider.Recommend(context);
                }
                catch (Exception ex)
                {
                    _logger.Error("Provider " + provider.GetType().FullName + " failed.", ex);
                }

            }

            return items.OrderByDescending(x => x.Score).Take(10).ToList();
        }
    }
}