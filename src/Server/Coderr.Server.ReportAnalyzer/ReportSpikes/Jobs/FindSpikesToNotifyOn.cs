using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Domain.Modules.UserNotifications;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.ReportSpikes.Entities;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Coderr.Server.ReportAnalyzer.UserNotifications.Dtos;
using DotNetCqs;
using Griffin.ApplicationServices;

namespace Coderr.Server.ReportAnalyzer.ReportSpikes.Jobs
{
    [ContainerService(RegisterAsSelf = true)]
    internal class FindSpikesToNotifyOn : IBackgroundJobAsync
    {
        private static readonly List<int> _notifiedApplicationsSafetyNet = new List<int>();
        private static DateTime _safetyNetDate = DateTime.Today;
        private readonly BaseConfiguration _baseConfiguration;
        private readonly IMessageBus _messageBus;
        private readonly INotificationService _notificationService;
        private readonly IUserNotificationsRepository _notificationsRepository;
        private readonly IReportSpikeRepository _repository;

        public FindSpikesToNotifyOn(IReportSpikeRepository repository, IConfiguration<BaseConfiguration> baseConfiguration,
            IUserNotificationsRepository notificationsRepository, IMessageBus messageBus,
            INotificationService notificationService)
        {
            _repository = repository;
            _baseConfiguration = baseConfiguration.Value;
            _notificationsRepository = notificationsRepository;
            _messageBus = messageBus;
            _notificationService = notificationService;
        }

        public async Task ExecuteAsync()
        {
            ClearSafetyNetIfNewDay();

            var aggregations = await _repository.GetWeeksAggregations();
            var aggregationsPerApplication = aggregations.GroupBy(x => x.ApplicationId);
            foreach (var appReports in aggregationsPerApplication)
            {
                var last = appReports.LastOrDefault();
                if (last == null || last.TrackedDate != DateTime.Today || last.Notified || last.ReportCount < last.Percentile85)
                    continue;

                // We have notified of this application recently. No need to do it again.
                if (appReports.Any(x => x.Notified))
                    continue;

                if (EnsureThatWeHaventNotifiedForTheApplication(last))
                    continue;

                await Notify(last);

            }
        }

        private Notification BuildBrowserNotification(SpikeAggregation spikeInfo, UserNotificationSettings setting)
        {
            var notification =
                new Notification(
                    $"Coderr have received {spikeInfo.ReportCount} reports so far. Day average is {spikeInfo.Percentile50}.")
                {
                    Title = $"Spike detected for {spikeInfo.ApplicationName}",
                    Actions =
                        new List<NotificationAction>
                        {
                            new NotificationAction {Title = "View", Action = "discoverApplication"}
                        },
                    Data = new
                    {
                        applicationId = spikeInfo.ApplicationId,
                        accountId = setting.AccountId,
                        discoverApplicationUrlk = $"{_baseConfiguration.BaseUrl}/discover/{spikeInfo.ApplicationId}"
                    },
                    Timestamp = DateTime.UtcNow
                };
            return notification;
        }

        private EmailMessage BuildEmail(SpikeAggregation spikeInfo, UserNotificationSettings setting)
        {
            var msg = new EmailMessage(setting.AccountId.ToString())
            {
                Subject = $"Spike detected for {spikeInfo.ApplicationName} ({spikeInfo} reports)",
                HtmlBody =
                    $"We've detected a spike in incoming reports for application <a href=\"{_baseConfiguration.BaseUrl}discover/{spikeInfo.ApplicationId}/\">{spikeInfo.ApplicationName}</a>\r\n" +
                    "\r\n" +
                    $"We've received {spikeInfo.ReportCount} reports so far. Day average is {spikeInfo.Percentile50}\r\n" +
                    "\r\n" + "No further notifications will be sent today for this application."
            };
            return msg;
        }

        private static void ClearSafetyNetIfNewDay()
        {
            if (_safetyNetDate.Date == DateTime.Today)
            {
                return;
            }

            _notifiedApplicationsSafetyNet.Clear();
            _safetyNetDate = DateTime.Today;
        }

        private static bool EnsureThatWeHaventNotifiedForTheApplication(SpikeAggregation violation)
        {
            if (_notifiedApplicationsSafetyNet.Contains(violation.ApplicationId)) return true;

            _notifiedApplicationsSafetyNet.Add(violation.ApplicationId);
            return false;
        }

        private async Task Notify(SpikeAggregation spikeAggregation)
        {
            var notificationSettings =
                (await _notificationsRepository.GetAllAsync(spikeAggregation.ApplicationId)).ToList();
            if (notificationSettings.All(x => x.ApplicationSpike == NotificationState.Disabled))
                return;

            spikeAggregation.Notified = true;
            await _repository.MarkAsNotified(spikeAggregation.Id);

            foreach (var setting in notificationSettings)
            {
                if (setting.ApplicationSpike == NotificationState.Disabled)
                    continue;

                if (setting.ApplicationSpike == NotificationState.Email)
                {
                    var msg = BuildEmail(spikeAggregation, setting);
                    var cmd = new SendEmail(msg);
                    await _messageBus.SendAsync(cmd);
                }
                else
                {
                    var notification = BuildBrowserNotification(spikeAggregation, setting);
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
        }
    }
}