using System;
using System.Collections.Generic;
using System.Text;
using Coderr.Server.ReportAnalyzer.ReportSpikes.Entities;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Spikes
{
    class SpikeAggregationMapper : CrudEntityMapper<SpikeAggregation>
    {
        public SpikeAggregationMapper() : base("SpikeAggregation")
        {
        }
    }
}
