using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Coderr.Server.Domain.Modules.Similarities
{
    /// <summary>
    ///     Stores information about all context collection properties and how often their different values are the same.
    /// </summary>
    /// <remarks>
    ///     For instance. 90 reports may have v4.0.0 of the assembly Framework.Core while 10 reports have v3.5.0. that means
    ///     that v4.0.0 is used in 90% of the reports and therefore the
    ///     assembly that the support team should use when trying to find the exception.
    /// </remarks>
    public class SimilaritiesReport
    {
        private static readonly string[] IgnoredCollections = {"OpenForms", "Screenshots"};
        private readonly List<SimilarityCollection> _collections = new List<SimilarityCollection>();


        /// <summary>
        ///     Creates a new instance of <see cref="SimilaritiesReport" />.
        /// </summary>
        /// <param name="incidentId">Incident that this report belongs to</param>
        /// <exception cref="ArgumentNullException">incidentId</exception>
        public SimilaritiesReport(int incidentId)
        {
            if (incidentId < 1) throw new ArgumentNullException("incidentId");
            IncidentId = incidentId;
            ReportCount = 0;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected SimilaritiesReport()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SimilaritiesReport" />.
        /// </summary>
        /// <param name="incidentId">Incident that this report belongs to</param>
        /// <param name="collections">all generated collections</param>
        /// <exception cref="ArgumentNullException">collections</exception>
        /// <exception cref="ArgumentOutOfRangeException">incidentId</exception>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public SimilaritiesReport(int incidentId, List<SimilarityCollection> collections)
        {
            if (collections == null) throw new ArgumentNullException("collections");
            if (incidentId <= 0) throw new ArgumentOutOfRangeException("incidentId");
            IncidentId = incidentId;
            _collections = collections;
        }

        /// <summary>
        ///     Collections
        /// </summary>
        public IEnumerable<SimilarityCollection> Collections
        {
            get { return _collections; }
        }

        /// <summary>
        ///     Incident that this is a analysis for.
        /// </summary>
        public int IncidentId { get; private set; }

        /// <summary>
        ///     Number of reports for the incident
        /// </summary>
        public int ReportCount { get; private set; }

        /// <summary>
        ///     Get a specific collection
        /// </summary>
        /// <param name="contextName">context collection name</param>
        /// <returns>collection if found; otherwise <c>null</c>.</returns>
        protected SimilarityCollection GetCollection(string contextName)
        {
            if (contextName == null) throw new ArgumentNullException("contextName");

            return _collections.FirstOrDefault(x => x.Name.Equals(contextName, StringComparison.OrdinalIgnoreCase));
        }

        public void AddSimilarity(string contextName, string propertyName, object adaptedValue)
        {
            var collection = GetCollection(contextName);
            if (collection == null)
            {
                collection = new SimilarityCollection(IncidentId, contextName);
                _collections.Add(collection);
            }

            collection.Add(propertyName, adaptedValue);
        }

        public void IncreateReportCount()
        {
            ReportCount += 1;
        }

        public bool IsIgnored(string contextName)
        {
            return IgnoredCollections.Contains(contextName, StringComparer.OrdinalIgnoreCase);
        }
    }
}