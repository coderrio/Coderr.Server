using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coderr.Server.Domain.Modules.Similarities;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.Similarities.Adapters.Runner;
using Coderr.Server.ReportAnalyzer.Similarities.Handlers.Processing;
using DotNetCqs;
using Coderr.Server.Abstractions.Boot;
using log4net;

namespace Coderr.Server.ReportAnalyzer.Similarities.Handlers
{
    /// <summary>
    ///     Responsible of analyzing the reports Context Data to find similarities from all reports in an incident.
    /// </summary>
    public class UpdateSimilaritiesFromNewReport : IMessageHandler<ReportAddedToIncident>
    {
        private readonly AdapterRepository _adapterRepository = new AdapterRepository();
        private readonly ILog _logger = LogManager.GetLogger(typeof(UpdateSimilaritiesFromNewReport));
        private readonly ISimilarityRepository _similarityRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdateSimilaritiesFromNewReport" />.
        /// </summary>
        /// <param name="similarityRepository">repository</param>
        /// <exception cref="ArgumentNullException">similarityRepository</exception>
        public UpdateSimilaritiesFromNewReport(ISimilarityRepository similarityRepository)
        {
            _similarityRepository = similarityRepository ?? throw new ArgumentNullException(nameof(similarityRepository));
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            if (e.IsStored != true)
            {
                return;
            }

            _logger.Debug("Updating similarities " + e.Incident.Id);
            var adapters = _adapterRepository.GetAdapters();
            var sw2 = new Stopwatch();
            sw2.Start();
            long beginStep, afterFindStep, afterReposStep;

            try
            {
                _logger.Debug("Finding for incident: " + e.Incident.Id);
                beginStep = sw2.ElapsedMilliseconds;

                var similaritiesReport = _similarityRepository.FindForIncident(e.Incident.Id);
                var isNew = false;
                if (similaritiesReport == null)
                {
                    similaritiesReport = new SimilaritiesReport(e.Incident.Id);
                    isNew = true;
                }

                var analyzer = new SimilarityAnalyzer(similaritiesReport);
                afterFindStep = sw2.ElapsedMilliseconds;
                analyzer.AddReport(e.Report, adapters);

                if (isNew)
                {
                    _logger.Debug("Creating new similarity report...");
                    await _similarityRepository.CreateAsync(similaritiesReport);
                }
                else
                {
                    _logger.Debug("Updating existing similarity report...");
                    await _similarityRepository.UpdateAsync(similaritiesReport);
                }

                afterReposStep = sw2.ElapsedMilliseconds;
                _logger.Debug("similarities done ");
            }
            catch (Exception exception)
            {
                _logger.Error("failed to add report to incident " + e.Incident.Id, exception);

                // Live changes since we get deadlocks?
                // TODO: WHY do we get deadlocks, aren't we the only ones reading from the similarity tables?
                return;
            }
            sw2.Stop();
            if (sw2.ElapsedMilliseconds > 200)
            {
                _logger.InfoFormat("Slow similarity handling, times: {0}/{1}/{2}/{3}", beginStep, afterFindStep, afterReposStep,
                    sw2.ElapsedMilliseconds);
            }
        }
    }
}