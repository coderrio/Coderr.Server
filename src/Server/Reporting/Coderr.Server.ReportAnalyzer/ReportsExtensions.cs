using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;

namespace Coderr.Server.ReportAnalyzer
{
    public static class ReportsExtensions
    {
        public static ErrorReportContextCollection GetCoderrCollection(
            this IEnumerable<ErrorReportContextCollection> instance)
        {
            return instance.FirstOrDefault(x => x.Name == "CoderrData");
        }

        public static ErrorReportContextCollection GetCoderrCollection(
            this ErrorReportEntity instance)
        {
            return instance.ContextCollections.FirstOrDefault(x => x.Name == "CoderrData");
        }


        public static ErrorReportContextCollection FindCollection(this ErrorReportEntity instance, string collectionName)
        {
            return instance.ContextCollections.FirstOrDefault(x => x.Name == collectionName);
        }

        public static string FindCollectionProperty(this ErrorReportEntity instance, string collectionName, string propertyName)
        {
            var collection = instance.FindCollection(collectionName);
            if (collection == null)
            {
                return null;
            }

            if (collection.Properties.TryGetValue(propertyName, out var value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }


        public static ContextCollectionDTO FindCollection(this ReportDTO instance, string collectionName)
        {
            return instance.ContextCollections.FirstOrDefault(x => x.Name == collectionName);
        }

        public static string FindCollectionProperty(this ReportDTO instance, string collectionName, string propertyName)
        {
            var collection = instance.FindCollection(collectionName);
            if (collection == null)
            {
                return null;
            }

            if (collection.Properties.TryGetValue(propertyName, out var value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public static string FindCollectionProperty(this ReportDTO instance, string propertyName)
        {
            foreach (var collection in instance.ContextCollections)
            {
                if (collection.Properties.TryGetValue(propertyName, out var value))
                {
                    return value;
                }
                
            }

            return null;
        }


    }
}
