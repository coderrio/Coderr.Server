using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Api.Insights.Queries;
using DotNetCqs;

namespace Coderr.Server.App.Insights.Metrics
{
    class GetKeyMetricHandler : IQueryHandler<GetKeyMetric, GetKeyMetricResult>
    {
        private IEnumerable<IKeyMetricGenerator> _generators;

        public GetKeyMetricHandler(IEnumerable<IKeyMetricGenerator> generators)
        {
            _generators = generators;
        }

        public Task<GetKeyMetricResult> HandleAsync(IMessageContext context, GetKeyMetric query)
        {
            throw new NotImplementedException();
        }
    }
}
