using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coderr.Server.Domain.Modules.Similarities;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.Similarities.Adapters.Runner;
using Coderr.Server.ReportAnalyzer.Similarities.Handlers.Processing;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
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
        /// <param name="similarityRepository">epos</param>
        /// <exception cref="ArgumentNullException">similarityReposiotry</exception>
        public UpdateSimilaritiesFromNewReport(ISimilarityRepository similarityRepository)
        {
            if (similarityRepository == null) throw new ArgumentNullException("similarityRepository");
            _similarityRepository = similarityRepository;
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
            _logger.Debug("Updating similarities");
            var adapters = _adapterRepository.GetAdapters();
            var sw2 = new Stopwatch();
            sw2.Start();
            long step1, step2, step3;

            try
            {
                _logger.Debug("Finding for incident: " + e.Incident.Id);
                var similaritiesReport = _similarityRepository.FindForIncident(e.Incident.Id);

                step1 = sw2.ElapsedMilliseconds;
                var isNew = false;
                if (similaritiesReport == null)
                {
                    _logger.Debug("Not found, creating a new");
                    similaritiesReport = new SimilaritiesReport(e.Incident.Id);
                    isNew = true;
                }

                var analyzer = new SimilarityAnalyzer(similaritiesReport);
                step2 = sw2.ElapsedMilliseconds;
                analyzer.AddReport(e.Report, adapters);

                if (isNew)
                {
                    _logger.Debug("Creating...");
                    await _similarityRepository.CreateAsync(similaritiesReport);
                }
                else
                {
                    _logger.Debug("Updating...");
                    await _similarityRepository.UpdateAsync(similaritiesReport);
                }

                step3 = sw2.ElapsedMilliseconds;
                _logger.Debug("similarities done ");
            }
            catch (Exception exception)
            {
                _logger.Error("failed to add report to incident " + e.Incident.Id, exception);
                throw;
            }
            sw2.Stop();
            if (sw2.ElapsedMilliseconds > 200)
            {
                _logger.InfoFormat("Slow similarity handling, times: {0}/{1}/{2}/{3}", step1, step2, step3,
                    sw2.ElapsedMilliseconds);
            }
        }
    }
}