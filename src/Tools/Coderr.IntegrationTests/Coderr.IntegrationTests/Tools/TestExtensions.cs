using System.Linq;
using Coderr.IntegrationTests.Core.TestFramework;
using Coderr.Server.Api.Core.Reports.Queries;

namespace Coderr.IntegrationTests.Core.Tools
{
    public static class TestExtensions
    {
        public static string CollectionProperty(this GetReportResult report, string collectionName, string propertyName)
        {
            var collection = report?.ContextCollections.FirstOrDefault(x => x.Name == collectionName);
            if (collection == null)
                throw new TestFailedException($"Failed to find collection {collectionName}");

            var kvp = collection.Properties.FirstOrDefault(x => x.Key == propertyName);
            if (kvp == null)
                throw new TestFailedException(
                    $"Failed to find property '{propertyName}' collection '{collectionName}'.");

            return kvp.Value;
        }

        public static string Property(this GetReportResultContextCollection collection, string propertyName)
        {
            var kvp = collection.Properties.FirstOrDefault(x => x.Key == propertyName);
            if (kvp == null)
                throw new TestFailedException($"Failed to find property '{propertyName}'.");

            return kvp.Value;
        }
    }
}