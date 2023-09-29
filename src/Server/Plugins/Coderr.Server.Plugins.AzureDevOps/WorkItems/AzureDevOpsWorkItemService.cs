using System;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using Coderr.Server.Common.AzureDevOps.App.Connections;
using Coderr.Server.Domain;
using log4net;

namespace Coderr.Server.Common.AzureDevOps.App.WorkItems
{
    //[ContainerService(RegisterAsSelf = true)]
    // Registered by Common/Boot.
    class AzureDevOpsWorkItemService : IWorkItemService
    {
        private readonly ISettingsRepository _configurationRepository;
        private readonly IWorkItemClient _workItemClient;
        public static string NAME = "AzureDevOps";
        private readonly ILog _logger = LogManager.GetLogger(typeof(AzureDevOpsWorkItemService));
        private readonly IUserMappingRepository _userMappingRepository;

        // For runtime
        public AzureDevOpsWorkItemService(ISettingsRepository configurationRepository, IUserMappingRepository userMappingRepository)
        {
            _configurationRepository = configurationRepository;
            _userMappingRepository = userMappingRepository;
        }

        // For tests
        public AzureDevOpsWorkItemService(ISettingsRepository configurationRepository, IWorkItemClient workItemClient, IUserMappingRepository userMappingRepository)
        {
            _configurationRepository = configurationRepository;
            _workItemClient = workItemClient;
            _userMappingRepository = userMappingRepository;
        }

        public string Name { get; } = NAME;
        public string Title { get; } = "Azure DevOps";

        public async Task Create(WorkItemDTO workItem)
        {
            var settings = await _configurationRepository.Get(workItem.ApplicationId);

            _logger.Debug("Creating work item type " + settings.WorkItemTypeName);
            var client = _workItemClient ?? new WorkItemClient(settings);
            var dto = await client.Create(workItem, settings.AreaPath);
            workItem.WorkItemId = dto.Id.Value.ToString();

            // api: https://dev.azure.com/1tcompany/75570083-b1ef-4e78-88e2-5db4982f756c/_apis/wit/workItems/310
            // api: https://dev.azure.com/fabrikam/Phone%20Saver/_workitems/edit/390
            // ui: https://(organizationname).visualstudio.com/(project)/_workitems/edit/(id)/
            // ui: https://OrganizationName.visualstudio.com/DefaultCollection/ProjectName/_workitems/edit/WorkItemNumber
            // ui: https://fabrikam/DefaultCollection/Phone%20Saver/_workitems/edit/390
            // ui: https://1tcompany.visualstudio.com/codeRR%20OSS%20Builds/_workitems/edit/311/

            //TODO: Premise support

            var apiPos = dto.Url.IndexOf("_apis/", 0, StringComparison.OrdinalIgnoreCase);
            var basePath = dto.Url.Substring(0, apiPos)+1;
            var uiUrl = $"{basePath}/_workitems/edit/{workItem.WorkItemId}/";

            workItem.ApiUrl = dto.Url;
            workItem.UiUrl = uiUrl;
            workItem.IntegrationName = NAME;
        }


        public async Task Assign(int applicationId, string workItemId, int accountId)
        {
            var settings = await _configurationRepository.Get(applicationId);
            var client = _workItemClient ?? new WorkItemClient(settings);

            var userMapping = await _userMappingRepository.Get(accountId);
            if (userMapping == null)
            {
                _logger.Error($"Failed to find account {accountId} which is linked to work item {workItemId}.");
                return;
            }

            var workItemIdValue = int.Parse(workItemId);
            var item = await client.Get(workItemIdValue);
            if (item == null)
            {
                throw new EntityNotFoundException("Failed to find Azure DevOps work item  " + workItemId);
            }

            var field = (string)item.Fields[Fields.State];
            if (field == settings.AssignedStateName)
                return;

            var name = userMapping.AdditionalData["UniqueName"];
            await client.Assign(workItemIdValue, name);
        }

        public async Task Solve(int applicationId, string workItemId, int accountId, string solution, string version)
        {
            var settings = await _configurationRepository.Get(applicationId);
            var client = _workItemClient ?? new WorkItemClient(settings);
            var userMapping = await _userMappingRepository.Get(accountId);
            if (userMapping == null)
            {
                _logger.Error($"Failed to find account {accountId} which is linked to work item {workItemId}.");
                return;
            }

            var workItemIdValue = int.Parse(workItemId);
            var item = await client.Get(workItemIdValue);
            if (item == null)
            {
                throw new EntityNotFoundException("Failed to find Azure DevOps work item  " + workItemId);
            }

            var field = (string)item.Fields[Fields.State];
            if (field == settings.SolvedStateName && field == settings.ClosedStateName)
            {
                return;
            }

            var name = userMapping.AdditionalData["UniqueName"];
            await client.Solve(int.Parse(workItemId), name, solution, version);
        }
    }
}
