using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.ReportAnalyzer.ReportSpikes.Entities;

namespace Coderr.Server.ReportAnalyzer.ReportSpikes
{
    public interface IReportSpikeRepository
    {
        Task<IReadOnlyList<SpikeAggregation>> GetWeeksAggregations();
        Task IncreaseReportCount(int applicationId, DateTime when);

        Task MarkAsNotified(int spikeId);
    }
}