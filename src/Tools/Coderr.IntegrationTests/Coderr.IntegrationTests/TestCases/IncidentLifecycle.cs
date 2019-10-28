using System.Linq;
using System.Threading.Tasks;
using Coderr.Client.ContextCollections;
using Coderr.IntegrationTests.Core.TestFramework;
using Coderr.IntegrationTests.Core.Tools;
using FluentAssertions;

namespace Coderr.IntegrationTests.Core.TestCases
{
    public class IncidentLifecycle
    {
        private readonly ApplicationClient _applicationClient;

        public IncidentLifecycle(ApplicationClient applicationClient)
        {
            _applicationClient = applicationClient;
        }

        [Test]
        public async Task Basic_flow_should_work()
        {
            var incident = await _applicationClient.CreateIncident();
            incident.Environments.Should().Be("IntegrationTests");
            await incident.Assign(1);
            await incident.Close(1, "Fixed it", "1.1.0");
            await incident.Report();
            await incident.UpdateByLastReceivedReport();
            incident.IsClosed.Should().BeTrue();
        }

        [Test]
        public async Task Should_attach_context_data()
        {
            var incident = await _applicationClient.CreateIncident(new {helloWorld = true});
            var report = await incident.GetReport();
            var data = report.ContextCollections.First(x => x.Name == "ContextData");
            data.Properties.First(x => x.Key == "helloWorld").Value.Should().Be("True");
        }

        [Test]
        public async Task Should_reopen_when_version_is_larger()
        {
            var incident = await _applicationClient.CreateIncident();
            await incident.Assign(1);
            await incident.Close(1, "Fixed it", "1.1.0");
            await incident.Report(x =>
            {
                var col = x.ContextCollections.GetCoderrCollection();
                col.Properties["AppAssemblyVersion"] = "1.1.2";
            });
            await incident.UpdateByLastReceivedReport();
            incident.IsClosed.Should().BeFalse();
        }
    }
}