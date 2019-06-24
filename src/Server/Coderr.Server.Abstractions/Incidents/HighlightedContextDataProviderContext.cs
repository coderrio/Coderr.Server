using System;
using System.Collections.Generic;
using Coderr.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.Abstractions.Incidents
{
    public class HighlightedContextDataProviderContext
    {
        private readonly IList<HighlightedContextData> _items;

        public HighlightedContextDataProviderContext(IList<HighlightedContextData> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            Tags = new string[0];
        }

        public int ApplicationId { get; set; }
        public string Description { get; set; }

        /// <summary>
        ///     Namespace + name of exception
        /// </summary>
        public string FullName { get; set; }

        public int IncidentId { get; set; }

        public IEnumerable<HighlightedContextData> Items => _items;

        public string StackTrace { get; set; }
        public string[] Tags { get; set; }

        public void AddValue(HighlightedContextData contextData)
        {
            if (contextData == null) throw new ArgumentNullException(nameof(contextData));
            _items.Add(contextData);
        }
    }
}