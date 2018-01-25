using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Events;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Versions.Events
{
    [Component]
    internal class GetVersionFromReport : IMessageHandler<ReportAddedToIncident>
    {
        public const string AppAssemblyVersion = "AppAssemblyVersion";
        private readonly IVersionRepository _repository;

        public GetVersionFromReport(IVersionRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            string version = null;
            foreach (var contextCollection in e.Report.ContextCollections)
            {
                if (!contextCollection.Properties.TryGetValue(AppAssemblyVersion, out version))
                    continue;
            }

            if (version == null)
                return;

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


            _repository.SaveIncidentVersion(e.Incident.Id, versionEntity.Id);

            await IncreaseReportCounter(versionEntity.Id, isNewIncident);
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