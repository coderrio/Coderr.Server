using System;
using System.Collections.Generic;
using System.Linq;

namespace Coderr.Server.App.Modules.Mine
{
    /// <summary>
    ///     Context for <see cref="IRecommendationProvider" />
    /// </summary>
    public class RecommendIncidentContext
    {
        private readonly List<RecommendedIncident> _items;

        public RecommendIncidentContext(List<RecommendedIncident> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        ///     User that want a suggestion
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        ///     If the user have selected a specific application
        /// </summary>
        public int? ApplicationId { get; set; }

        /// <summary>
        /// Number of items each provider should suggest.
        /// </summary>
        public int NumberOfItems { get; set; } = 10;

        /// <summary>
        ///     Add another item.
        /// </summary>
        /// <param name="suggestion"></param>
        /// <param name="scoreMultiplier">Used by providers that are worth more. 1 = equally worth. Corresponds to "Weight" for partitions. At most 10.</param>
        public void Add(RecommendedIncident suggestion, double scoreMultiplier)
        {
            if (suggestion == null) throw new ArgumentNullException(nameof(suggestion));

            // Should only suggest the same incident once.
            // thus if there are multiple suggestions for the same incident, increase the importance
            // and include both motivations.
            var first = _items.FirstOrDefault(x => x.IncidentId == suggestion.IncidentId);
            if (first != null)
            {
                first.Score += (int)(suggestion.Score * scoreMultiplier);
                first.Motivation += "\r\n" + suggestion.Motivation;
                return;
            }

            _items.Add(suggestion);
        }
    }
}