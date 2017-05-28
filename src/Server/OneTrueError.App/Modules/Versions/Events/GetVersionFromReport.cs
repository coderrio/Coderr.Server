using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Incidents.Events;
using OneTrueError.Api.Core.Notifications;
using OneTrueError.App.Modules.Versions.Config;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Infrastructure.Security;

namespace OneTrueError.App.Modules.Versions.Events
{
    [Component]
    internal class GetVersionFromReport : IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private const string NotifyNoVersion = "Versions.Configure";
        private const string NotifyNotRecognizedVersion = "Versions.ReConfigure";
        private readonly ICommandBus _commandBus;
        private IVersionRepository _repository;

        public GetVersionFromReport(ICommandBus commandBus, IVersionRepository repository)
        {
            _commandBus = commandBus;
            _repository = repository;
        }

        public async Task HandleAsync(ReportAddedToIncident e)
        {
            var assemblyName = GetVersionAssemblyName(e.Incident.ApplicationId);
            if (assemblyName == null)
            {
                var notice = new AddNotification(OneTrueClaims.RoleSysAdmin,
                    "There is no version assembly configured for " + e.Incident.ApplicationName +
                    ". Go to 'System Settings'/Versions and configure one");
                notice.HoldbackInterval = TimeSpan.FromDays(3);
                notice.NotificationType = NotifyNoVersion;
                await _commandBus.ExecuteAsync(notice);
                return;
            }

            var collection = e.Report.ContextCollections.FirstOrDefault(x => x.Name == "Assemblies");
            if (collection == null)
                return;

            string version;
            if (!collection.Properties.TryGetValue(assemblyName, out version))
            {
                var notice = new AddNotification(OneTrueClaims.RoleSysAdmin,
                    "Assembly " + assemblyName + " is configured for application " + e.Incident.ApplicationName +
                    ". It do however not exist. Configure a new one at 'System Settings'/Versions.");
                notice.HoldbackInterval = TimeSpan.FromDays(3);
                notice.NotificationType = NotifyNotRecognizedVersion;
                await _commandBus.ExecuteAsync(notice);
                return;
            }

            var isNewIncident = e.Incident.ReportCount <= 1;
            var versionEntity = await _repository.GetVersionAsync(e.Incident.ApplicationId, version)
                                ?? new ApplicationVersion(e.Incident.ApplicationId, e.Incident.ApplicationName,
                                    version);
            if (versionEntity.Version != version)
                versionEntity = new ApplicationVersion(e.Incident.ApplicationId, e.Incident.ApplicationName, version);
            versionEntity.UpdateReportDate();

            if (versionEntity.Id == 0)
                await _repository.CreateAsync(versionEntity);
            else
                await _repository.UpdateAsync(versionEntity);

            await IncreaseReportCounter(versionEntity.Id, isNewIncident);
        }

        private string GetVersionAssemblyName(int applicationId)
        {
            var config = ConfigurationStore.Instance.Load<ApplicationVersionConfig>();
            return config == null ? null : config.GetAssemblyName(applicationId);
        }

        private async Task IncreaseReportCounter(int versionId, bool isNewIncident)
        {
            var month =
                await _repository.GetMonthForApplicationAsync(versionId, DateTime.Today.Year, DateTime.Today.Month) ??
                new ApplicationVersionMonth(versionId, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));

            if (isNewIncident)
                month.IncreaseIncidentCount();
            else
                month.IncreaseReportCount();

            if (month.Id == 0)
                await _repository.CreateAsync(month);

            else
                await _repository.UpdateAsync(month);
        }
    }
}