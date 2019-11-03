using System.IO;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.TestCases;
using Coderr.Tests.Runners;
using Coderr.Tests.Runners.AppDomain.DTO;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cmd = new RunTests()
            {
                AssemblyName = typeof(EnvironmentTests).Assembly.GetName().Name,
                AssemblyPath = Path.GetDirectoryName(typeof(EnvironmentTests).Assembly.Location),
                TestCases = new TestCaseToRun[]
                {
                    new TestCaseToRun("Coderr.IntegrationTests.Core.TestCases.EnvironmentTests.Clearing_environment_should_remove_all_incidents_in_it")
                    {
                        TestClassName = "EnvironmentTests",
                        TestMethodName = "Clearing_environment_should_remove_all_incidents_in_it"
                    },
                },
                Source = typeof(EnvironmentTests).Assembly.Location
            };

            var receiver = new ConsoleReceiver();
            using (var coordinator = new AppDomainRunner("TesTSuite1"))
            {
                coordinator.Create();
                coordinator.AddDirectory(
                    @"C:\src\1tcompany\coderr\oss\Coderr.Server\src\Tools\Coderr.IntegrationTests\Coderr.IntegrationTests\bin\Debug\net461");
                coordinator.RunTests(cmd, receiver);
            }
        }
    }
}
