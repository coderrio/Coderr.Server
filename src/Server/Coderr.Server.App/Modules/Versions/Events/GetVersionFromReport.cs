using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Events;
using codeRR.Server.Api.Core.Notifications;
using codeRR.Server.App.Modules.Versions.Config;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Infrastructure.Security;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Versions.Events
{
    [Component]
    internal class GetVersionFromReport : IMessageHandler<ReportAddedToIncident>
    {
        private const string NotifyNoVersion = "Versions.Configure";
        private const string NotifyNotRecognizedVersion = "Versions.ReConfigure";
        private IVersionRepository _repository;
        private ConfigurationStore _configStore;

        public GetVersionFromReport(IVersionRepository repository, ConfigurationStore configStore)
        {
            _repository = repository;
            _configStore = configStore;
        }

        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            var assemblyName = GetVersionAssemblyName(e.Incident.ApplicationId);
            if (assemblyName == null)
            {
                var notice = new AddNotification(CoderrClaims.RoleSysAdmin,
                    "There is no version assembly configured for " + e.Incident.ApplicationName +
                    ". Go to 'System Settings'/Versions and configure one")
                {
                    HoldbackInterval = TimeSpan.FromDays(3),
                    NotificationType = NotifyNoVersion
                };
                await context.SendAsync(notice);
                return;
            }

            var collection = Enumerable.FirstOrDefault(e.Report.ContextCollections, x => x.Name == "Assemblies");
            if (collection == null)
                return;

            if (!collection.Properties.TryGetValue(assemblyName, out string version))
            {
                var notice = new AddNotification(CoderrClaims.RoleSysAdmin,
                    "Assembly " + assemblyName + " is configured for application " + e.Incident.ApplicationName +
                    ". It do however not exist. Configure a new one at 'System Settings'/Versions.")
                {
                    HoldbackInterval = TimeSpan.FromDays(3),
                    NotificationType = NotifyNotRecognizedVersion
                };
                await context.SendAsync(notice);
                return;
            }

            var isNewIncident = e.Incident.ReportCount <= 1;
            var versionEntity = await _repository.FindVersionAsync(e.Incident.ApplicationId, version)
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
            var config = _configStore.Load<ApplicationVersionConfig>();
            return config?.GetAssemblyName(applicationId);
        }

        private async Task IncreaseReportCounter(int versionId, bool isNewIncident)
        {
            var month =
                await _repository.FindMonthForApplicationAsync(versionId, DateTime.Today.Year, DateTime.Today.Month) ??
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