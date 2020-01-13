using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Domain.Modules.ReportSpikes;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using Coderr.Server.ReportAnalyzer.UserNotifications.Handlers;
using DotNetCqs;
using log4net;
using Newtonsoft.Json;

namespace Coderr.Server.ReportAnalyzer.ReportSpikes.Handlers
{
    /// <summary>
    /// </summary>
    public class CheckForReportSpike : IMessageHandler<ReportAddedToIncident>
    {
        private readonly IUserNotificationsRepository _repository;
        private readonly IReportSpikeRepository _spikeRepository;
        private readonly BaseConfiguration _baseConfiguration;
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Creates a new instance of <see cref="CheckForReportSpike" />.
        /// </summary>
        /// <param name="repository">To check if spikes should be analyzed</param>
        /// <param name="spikeRepository">store/fetch information of current spikes.</param>
        /// <param name="baseConfiguration"></param>
        public CheckForReportSpike(IUserNotificationsRepository repository, IReportSpikeRepository spikeRepository, IConfiguration<BaseConfiguration> baseConfiguration, INotificationService notificationService)
        {
            _repository = repository;
            _spikeRepository = spikeRepository;
            _notificationService = notificationService;
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

            var notificationSettings = (await _repository.GetAllAsync(e.Report.ApplicationId)).ToList();
            if (notificationSettings.All(x => x.ApplicationSpike == NotificationState.Disabled))
                return;

            var countToday = await CalculateSpike(e);
            if (countToday == null)
                return;

            var spike = await _spikeRepository.GetSpikeAsync(e.Incident.ApplicationId);
            spike?.IncreaseReportCount();

            var existed = spike != null;
            foreach (var setting in notificationSettings)
            {
                if (setting.ApplicationSpike == NotificationState.Disabled)
                    continue;

                if (spike != null && spike.HasAccount(setting.AccountId))
                    continue;

                if (spike == null)
                    spike = new ErrorReportSpike(e.Incident.ApplicationId, 1);

                spike.AddNotifiedAccount(setting.AccountId);

                if (setting.ApplicationSpike == NotificationState.Email)
                {
                    var msg = BuildEmail(e, setting, countToday);
                    var cmd = new SendEmail(msg);
                    await context.SendAsync(cmd);
                }
                else
                {
                    var notification = BuildBrowserNotification(e, countToday, setting);
                    try
                    {
                        await _notificationService.SendBrowserNotification(setting.AccountId, notification);
                    }
                    catch (Exception ex)
                    {
                        Err.Report(ex, new {setting, notification});
                    }
                }
            }

            if (existed)
                await _spikeRepository.UpdateSpikeAsync(spike);
            else
                await _spikeRepository.CreateSpikeAsync(spike);
        }

        private Notification BuildBrowserNotification(ReportAddedToIncident e, NewSpike countToday,
            UserNotificationSettings setting)
        {
            var notification =
                new Notification(
                    $"Coderr have received {countToday.SpikeCount} reports so far. Day average is {countToday.DayAverage}.")
                {
                    Title = $"Spike detected for {e.Incident.ApplicationName}",
                    Actions = new List<NotificationAction>
                    {
                        new NotificationAction
                        {
                            Title = "View",
                            Action = "discoverApplication"
                        }
                    },
                    Data = new
                    {
                        applicationId = e.Incident.ApplicationId,
                        accountId = setting.AccountId,
                        discoverApplicationUrlk = $"{_baseConfiguration.BaseUrl}/discover/{e.Incident.ApplicationId}"
                    },
                    Timestamp = e.Report.CreatedAtUtc
                };
            return notification;
        }

        private EmailMessage BuildEmail(ReportAddedToIncident e, UserNotificationSettings setting, NewSpike countToday)
        {
            var msg = new EmailMessage(setting.AccountId.ToString())
            {
                Subject = $"Spike detected for {e.Incident.ApplicationName} ({countToday} reports)",
                HtmlBody =
                    $"We've detected a spike in incoming reports for application <a href=\"{_baseConfiguration.BaseUrl}discover/{e.Incident.ApplicationId}/\">{e.Incident.ApplicationName}</a>\r\n" +
                    "\r\n" +
                    $"We've received {countToday.SpikeCount} reports so far. Day average is {countToday.DayAverage}\r\n" +
                    "\r\n" + "No further spike emails will be sent today for this application."
            };
            return msg;
        }

        /// <summary>
        ///     Compare received amount of report with a calculated threshold.
        /// </summary>
        /// <param name="applicationEvent">e</param>
        /// <returns>-1 if no spike is detected; otherwise the spike count</returns>
        protected async Task<NewSpike> CalculateSpike(ReportAddedToIncident applicationEvent)
        {
            if (applicationEvent == null) throw new ArgumentNullException(nameof(applicationEvent));

            var average = await _spikeRepository.GetAverageReportCountAsync(applicationEvent.Incident.ApplicationId);
            if (average == 0)
                return null;

            var todaysCount = await _spikeRepository.GetTodaysCountAsync(applicationEvent.Incident.ApplicationId);
            var threshold = average > 100 ? average * 1.2 : average * 2;
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