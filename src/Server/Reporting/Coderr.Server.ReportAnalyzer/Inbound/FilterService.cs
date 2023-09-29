using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.Incidents;
using log4net;

namespace Coderr.Server.ReportAnalyzer.Inbound
{
    /// <summary>
    ///     Uses a bunch of filters to determine of the inbound error report should be analyzed (and/or being processed by
    ///     events internally)
    /// </summary>
    [ContainerService]
    public class FilterService : IFilterService
    {
        private readonly IReadOnlyList<IReportFilter> _filters;
        private readonly ILog _logger = LogManager.GetLogger(typeof(FilterService));


        public FilterService(IEnumerable<IReportFilter> filters)
        {
            _filters = filters.ToList();
        }

        public async Task<FilterResult> CanProcess(ErrorReportEntity report, IncidentBeingAnalyzed incident)
        {
            var recommendedAction = FilterResult.FullAnalyzis;
            var context = new FilterContext {ErrorReport = report};
            foreach (var filter in _filters)
            {
                var result = await filter.Filter(context);
                if (result == FilterResult.FullAnalyzis)
                {
                    continue;
                }

                _logger.Debug("Filter " + filter + " want to " + result);

                // We want to pick the worst recommendation.
                if (result == FilterResult.DiscardReport)
                {
                    recommendedAction = FilterResult.DiscardReport;
                }
                else if (recommendedAction == FilterResult.FullAnalyzis)
                {
                    recommendedAction = FilterResult.ProcessAndDiscard;
                }
            }

            return recommendedAction;
        }
    }
}