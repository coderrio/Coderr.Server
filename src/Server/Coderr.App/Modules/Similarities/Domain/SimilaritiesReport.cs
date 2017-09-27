using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner;

namespace codeRR.Server.App.Modules.Similarities.Domain
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
        ///     Add a new report.
        /// </summary>
        /// <param name="report">report</param>
        /// <param name="adapters">adapters</param>
        public void AddReport(ReportDTO report, IReadOnlyList<IValueAdapter> adapters)
        {
            if (report == null) throw new ArgumentNullException("report");
            if (adapters == null) throw new ArgumentNullException("adapters");

            //TODO: Varför skicka in adapters? Skapa via en Factory istället
            if (report == null) throw new ArgumentNullException("report");

            ReportCount += 1;

            foreach (var context in report.ContextCollections)
            {
                if (context.Name == null)
                    throw new ArgumentException("ContextInfo.Name may not be null.");

                if (IgnoredCollections.Contains(context.Name, StringComparer.OrdinalIgnoreCase))
                    continue;

                foreach (var property in context.Properties)
                {
                    if (property.Value != null && property.Value.Length > 40)
                        continue;

                    if (property.Key.Equals("OEMStringArray"))
                        continue;
                    if (context.Name.Equals("ExceptionProperties") && property.Key == "Message")
                        continue;
                    if (context.Name.Equals("ExceptionProperties") && property.Key == "StackTrace")
                        continue;
                    if (context.Name.Equals("ExceptionProperties") && property.Key == "InnerException")
                        continue;
                    if (property.Key.Contains("LastModified"))
                        continue;
                    if (property.Key == "Id")
                        continue;

                    var adapterContext = new ValueAdapterContext(context.Name, property.Key, property.Value, report);
                    object adaptedValue = property.Value;
                    foreach (var adapter in adapters)
                    {
                        adaptedValue = adapter.Adapt(adapterContext, adaptedValue);
                    }

                    foreach (var field in adapterContext.CustomFields)
                    {
                        AddSimilarity(field.ContextName, field.PropertyName, field.Value);
                    }

                    if (adapterContext.IgnoreProperty || "".Equals(adaptedValue))
                        continue;

                    if (adaptedValue == null)
                        adaptedValue = adapterContext.Value ?? "null";
                    AddSimilarity(context.Name, adapterContext.PropertyName, adaptedValue);
                    //similarity.IncreaseUsage(ReportCount);
                }
            }
        }

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

        private void AddSimilarity(string contextName, string propertyName, object adaptedValue)
        {
            var collection = GetCollection(contextName);
            if (collection == null)
            {
                collection = new SimilarityCollection(IncidentId, contextName);
                _collections.Add(collection);
            }

            collection.Add(propertyName, adaptedValue);
        }
    }
}