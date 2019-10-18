using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coderr.IntegrationTests.Core.TestCases
{
    public class IncidentLifecycle
    {
        private ApplicationClient _applicationClient;

        public IncidentLifecycle(ApplicationClient applicationClient)
        {
            _applicationClient = applicationClient;
        }

        public async Task Execute()
        {
            var incident = await _applicationClient.CreateIncident();
            await incident.Assign(1);
        }
    }
}
