using System;
using System.Collections.Generic;
using Coderr.Server.Domain.Modules.Similarities;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Similarities.Adapters;

namespace Coderr.Server.ReportAnalyzer.Similarities
{
    class SimilarityAnalyzer
    {
        private readonly SimilaritiesReport _similarity;

        public SimilarityAnalyzer(SimilaritiesReport similarity)
        {
            _similarity = similarity;
        }
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

            _similarity.IncreateReportCount();

            foreach (var context in report.ContextCollections)
            {
                if (context.Name == null)
                    throw new ArgumentException("ContextInfo.Name may not be null.");

                
                if (_similarity.IsIgnored(context.Name))
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
                        _similarity.AddSimilarity(field.ContextName, field.PropertyName, field.Value);
                    }

                    if (adapterContext.IgnoreProperty || "".Equals(adaptedValue))
                        continue;

                    if (adaptedValue == null)
                        adaptedValue = adapterContext.Value ?? "null";
                    _similarity.AddSimilarity(context.Name, adapterContext.PropertyName, adaptedValue);
                    //similarity.IncreaseUsage(ReportCount);
                }
            }
        }

    }
}
