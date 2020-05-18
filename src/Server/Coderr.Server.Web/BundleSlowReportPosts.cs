using System.Linq;
using Coderr.Client.Contracts;
using Coderr.Client.Processor;

namespace Coderr.Server.Web
{
    /// <summary>
    ///     HTTP client incidents are calculated using urls, remove the ErrorId from the url to group them.
    /// </summary>
    internal class BundleSlowReportPosts : IReportFilter
    {
        public void Invoke(ReportFilterContext context)
        {
            var collection = FindUrl(context);
            if (collection == null)
                return;

            var property = collection.Properties.FirstOrDefault(x => x.Key == "Url");
            var pos = property.Value.LastIndexOf('/');
            if (pos != -1) collection.Properties["Url"] = property.Value.Remove(pos);
        }

        private static ContextCollectionDTO FindUrl(ReportFilterContext context)
        {
            string url;
            return (
                from collection in context.Report.ContextCollections
                from property in collection.Properties
                where property.Key == "Url" && property.Value.Contains("/receiver/report/")
                select collection
            ).FirstOrDefault();
        }
    }
}