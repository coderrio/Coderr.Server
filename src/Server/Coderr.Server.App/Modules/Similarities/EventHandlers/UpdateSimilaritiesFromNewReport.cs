using System;
using System.Diagnostics;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Events;
using codeRR.Server.App.Modules.Similarities.Domain;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Modules.Similarities.EventHandlers
{
    /// <summary>
    ///     Responsible of analyzing the reports Context Data to find similarities from all reports in an incident.
    /// </summary>
    [Component(RegisterAsSelf = true)]
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
                var similarity = _similarityRepository.FindForIncident(e.Incident.Id);

                step1 = sw2.ElapsedMilliseconds;
                var isNew = false;
                if (similarity == null)
                {
                    _logger.Debug("Not found, creating a new");
                    similarity = new SimilaritiesReport(e.Incident.Id);
                    isNew = true;
                }

                step2 = sw2.ElapsedMilliseconds;
                similarity.AddReport(e.Report, adapters);

                if (isNew)
                {
                    _logger.Debug("Creating...");
                    await _similarityRepository.CreateAsync(similarity);
                }
                else
                {
                    _logger.Debug("Updating...");
                    await _similarityRepository.UpdateAsync(similarity);
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