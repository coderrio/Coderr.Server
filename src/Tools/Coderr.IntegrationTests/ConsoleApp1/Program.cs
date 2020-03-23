using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.Entities;
using Coderr.IntegrationTests.Core.TestCases;
using Coderr.Tests;
using Coderr.Tests.Runners;
using Coderr.Tests.Runners.AppDomain.DTO;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var discoverer = new TestDiscoverer();
            discoverer.Load(new[] {typeof(IncidentWrapper).Assembly});
            var runner = new TestRunner(discoverer);
            runner.Load(new[] {typeof(IncidentWrapper).Assembly}).GetAwaiter().GetResult();
            var result = runner.RunAll().GetAwaiter().GetResult();
            var faulty = result.Where(x => !x.IsSuccess).ToList();

            var cmd = new RunTests
            {
                AssemblyName = typeof(EnvironmentTests).Assembly.GetName().Name,
                AssemblyPath = Path.GetDirectoryName(typeof(EnvironmentTests).Assembly.Location),
                //TestCases = new[]
                //{
                //    new TestCaseToRun("Coderr.IntegrationTests.Core.TestCases.EnvironmentTests.Clearing_environment_should_remove_all_incidents_in_it")
                //    {
                //        TestClassName = "EnvironmentTests",
                //        TestMethodName = "Clearing_environment_should_remove_all_incidents_in_it"
                //    },
                //},
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
