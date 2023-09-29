using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Api.Escalation.Events;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Jobs
{
    internal class AddEscalatedErrorsToBacklog : IMessageHandler<IncidentEscalated>
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IWorkItemServiceProvider _workItemServiceProvider;

        public AddEscalatedErrorsToBacklog(IWorkItemServiceProvider workItemServiceProvider, ISettingsRepository settingsRepository)
        {
            _workItemServiceProvider = workItemServiceProvider;
            _settingsRepository = settingsRepository;
        }

        public async Task HandleAsync(IMessageContext context, IncidentEscalated message)
        {
            var settings = await _settingsRepository.Get(message.ApplicationId);
            if (settings?.AutoAddCritical == true && message.IsCritical)
                await AddIncident(message.IncidentId, message.ApplicationId);
            if (settings?.AutoAddImportant == true && message.IsImportant)
                await AddIncident(message.IncidentId, message.ApplicationId);
        }

        private async Task AddIncident(int incidentId, int applicationId)
        {
            await _workItemServiceProvider.CreateAsync(applicationId, incidentId);
        }
    }
}