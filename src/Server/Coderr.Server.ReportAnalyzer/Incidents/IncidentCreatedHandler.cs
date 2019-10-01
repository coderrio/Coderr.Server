using System.Threading.Tasks;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Core.Incidents.Events;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;

namespace Coderr.Server.ReportAnalyzer.Incidents
{
    class IncidentCreatedHandler : IMessageHandler<ReportAddedToIncident>
    {
        private IIncidentRepository _incidentRepository;

        public IncidentCreatedHandler(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident message)
        {
            if (message.IsNewIncident != true)
                return;
            var collection = message.Report.GetCoderrCollection();
            if (!collection.Properties.TryGetValue("CorrelationId", out var correlationId))
                return;

            await _incidentRepository.MapCorrelationId(message.Incident.Id, correlationId);
        }
    }
}
