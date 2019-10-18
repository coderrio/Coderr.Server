using System;
using System.Threading.Tasks;

namespace Coderr.IntegrationTests.Core
{
    class Program
    {
        public const string DbName = "Coderr99";
        public const string ServerAddress = "http://localhost:50473";

        static async Task Main(string[] args)
        {
            var apiClient = await CreateApiClient();
            var reporter = CreateReporter(ApplicationClient._applicationId);

            var report = reporter.ReportUnique("Hello world");
            reporter.ReportCopy(report);
            var incident = await apiClient.GetIncident(ApplicationClient._applicationId, "Hello world");

                
            Console.WriteLine("Hello World!");
        }
    }
}
