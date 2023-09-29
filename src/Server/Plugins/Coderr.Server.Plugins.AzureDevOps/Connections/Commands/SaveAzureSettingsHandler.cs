using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Common.AzureDevOps.Api.Connection.Commands;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.App.Connections.Commands
{
    class SaveAzureSettingsHandler: IMessageHandler<SaveAzureSettings>
    {
        private ISettingsRepository _settingsRepository;
        private IWorkItemServiceProvider _workItemServiceProvider;

        public SaveAzureSettingsHandler(ISettingsRepository settingsRepository, IWorkItemServiceProvider workItemServiceProvider)
        {
            _settingsRepository = settingsRepository;
            _workItemServiceProvider = workItemServiceProvider;
        }

        public async Task HandleAsync(IMessageContext context, SaveAzureSettings message)
        {
            var dbEntity = new Settings
            {
                CreatedById = context.Principal.GetAccountId(),
                ApplicationId = message.ApplicationId,
                AreaPath = message.AreaPath,
                AreaPathId = message.AreaPathId,
                PersonalAccessToken = message.PersonalAccessToken,
                ProjectId = message.ProjectId,
                ProjectName = message.ProjectName,
                Url = message.Url,
                AutoAddCritical = message.AutoAddCritical,
                AutoAddImportant = message.AutoAddImportant,
                AssignedStateName = message.AssignedStateName,
                ClosedStateName = message.ClosedStateName,
                SolvedStateName = message.SolvedStateName,
                //StateFieldName = message.StateFieldName,
                WorkItemTypeName = message.WorkItemTypeName
            };

            await _settingsRepository.Save(dbEntity);
            await _workItemServiceProvider.MapApplication(message.ApplicationId, "AzureDevOps");
        }
    }
}
