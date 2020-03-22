using System;
using DotNetCqs;

namespace Coderr.Server.Api.Core.Incidents.Queries
{
    /// <summary>
    ///     Fetch a specific collection from all reports, sorted in descending order.
    /// </summary>
    public class GetCollection : Query<GetCollectionResult>
    {
        public GetCollection(int incidentId, string collectionName)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            IncidentId = incidentId;
            CollectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
        }

        protected GetCollection()
        {
        }

        /// <summary>
        /// Collection name like "ErrorProperties" or "HttpRequest".
        /// </summary>
        public string CollectionName { get; private set; }

        /// <summary>
        /// Incident that the collection belongs to.
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        /// Collection limit.
        /// </summary>
        public int MaxNumberOfCollections { get; set; }
    }
}