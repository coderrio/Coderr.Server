using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;

namespace Coderr.Server.ReportAnalyzer.ReportSpikes.Handlers
{
    class IncreaseSpikeCountOnReportAdded : IMessageHandler<ReportAddedToIncident>
    {
        private IReportSpikeRepository _repository;

        public IncreaseSpikeCountOnReportAdded(IReportSpikeRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident message)
        {
            await _repository.IncreaseReportCount(message.Incident.ApplicationId, message.Report.CreatedAtUtc.Date);
        }
    }
}
