using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Domain.Modules.ReportSpikes;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.UserNotifications.Handlers;
using DotNetCqs;
using log4net;

namespace Coderr.Server.ReportAnalyzer.ReportSpikes.Handlers
{
    /// <summary>
    /// </summary>
    public class CheckForReportPeak : IMessageHandler<ReportAddedToIncident>
    {
        private readonly IUserNotificationsRepository _repository;
        private readonly IReportSpikeRepository _spikeRepository;
        private readonly BaseConfiguration _baseConfiguration;
        private ILog _log = LogManager.GetLogger(typeof(CheckForReportPeak));

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForReportPeak" />.
        /// </summary>
        /// <param name="repository">To check if spikes should be analyzed</param>
        /// <param name="spikeRepository">store/fetch information of current spikes.</param>
        /// <param name="baseConfiguration"></param>
        public CheckForReportPeak(IUserNotificationsRepository repository, IReportSpikeRepository spikeRepository, IConfiguration<BaseConfiguration> baseConfiguration)
        {
            _repository = repository;
            _spikeRepository = spikeRepository;
            _baseConfiguration = baseConfiguration.Value;
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
            if (e == null) throw new ArgumentNullException("e");

            _log.Info("ReportId: " + e.Report.Id);

            var url = _baseConfiguration.BaseUrl;
            var notificationSettings = (await _repository.GetAllAsync(e.Report.ApplicationId)).ToList();
            if (notificationSettings.All(x => x.ApplicationSpike == NotificationState.Disabled))
                return;

            var todaysCount = await CalculateSpike(e);
            if (todaysCount == null)
                return;

            var spike = await _spikeRepository.GetSpikeAsync(e.Incident.ApplicationId);
            if (spike != null)
                spike.IncreaseReportCount();

            var existed = spike != null;
            var messages = new List<EmailMessage>();
            foreach (var setting in notificationSettings)
            {
                if (setting.ApplicationSpike == NotificationState.Disabled)
                    continue;

                if (spike != null && spike.HasAccount(setting.AccountId))
                    continue;

                if (spike == null)
                    spike = new ErrorReportSpike(e.Incident.ApplicationId, 1);

                spike.AddNotifiedAccount(setting.AccountId);
                var msg = new EmailMessage(setting.AccountId.ToString())
                {
                    Subject = $"Spike detected for {e.Incident.ApplicationName} ({todaysCount} reports)",
                    HtmlBody =
                        $"We've detected a spike in incoming reports for application <a href=\"{url}/discover/{e.Incident.ApplicationId}/\">{e.Incident.ApplicationName}</a>\r\n" +
                        "\r\n" +
                        $"We've received {todaysCount.SpikeCount} reports so far. Day average is {todaysCount.DayAverage}\r\n" +
                        "\r\n" + "No further spike emails will be sent today for this application."
                };

                messages.Add(msg);
            }

            if (existed)
                await _spikeRepository.UpdateSpikeAsync(spike);
            else
                await _spikeRepository.CreateSpikeAsync(spike);

            foreach (var message in messages)
            {
                var sendEmail = new SendEmail(message);
                await context.SendAsync(sendEmail);
            }
        }

        /// <summary>
        ///     Compare received amount of report with a calculated threshold.
        /// </summary>
        /// <param name="applicationEvent">e</param>
        /// <returns>-1 if no spike is detected; otherwise the spike count</returns>
        protected async Task<NewSpike> CalculateSpike(ReportAddedToIncident applicationEvent)
        {
            if (applicationEvent == null) throw new ArgumentNullException("applicationEvent");

            var average = await _spikeRepository.GetAverageReportCountAsync(applicationEvent.Incident.ApplicationId);
            if (average == 0)
                return null;

            var todaysCount = await _spikeRepository.GetTodaysCountAsync(applicationEvent.Incident.ApplicationId);
            var threshold = average > 20 ? average * 1.2 : average * 2;
            if (todaysCount < threshold)
                return null;

            return new NewSpike
            {
                SpikeCount = todaysCount,
                DayAverage = average
            };
        }
    }
}