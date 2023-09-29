using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Api.Core.Incidents.Commands;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using Coderr.Server.Common.AzureDevOps.App.Connections;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;
using Griffin.ApplicationServices;
using log4net;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using ISettingsRepository = Coderr.Server.Common.AzureDevOps.App.Connections.ISettingsRepository;

namespace Coderr.Server.Common.AzureDevOps.App.WorkItems.Jobs
{
    [ContainerService(RegisterAsSelf = true)]
    internal class SynchronizeWorkItemsJob : IBackgroundJobAsync
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(SynchronizeWorkItemsJob));
        private readonly IMessageBus _messageBus;
        private readonly Dictionary<int, Settings> _settingsCache = new Dictionary<int, Settings>();
        private readonly ISettingsRepository _settingsRepository;
        private readonly IWorkItemRepository _workItemRepository;
        private readonly IUserMappingRepository _userMappingRepository;

        public SynchronizeWorkItemsJob(
            IWorkItemRepository workItemRepository,
            ISettingsRepository settingsRepository,
            IIncidentRepository incidentRepository,
            IMessageBus messageBus,
            IUserMappingRepository userMappingRepository)
        {
            _workItemRepository = workItemRepository;
            _settingsRepository = settingsRepository;
            _incidentRepository = incidentRepository;
            _messageBus = messageBus;
            _userMappingRepository = userMappingRepository;
        }

        public async Task ExecuteAsync()
        {
            await _workItemRepository.DeleteAbandonedWorkItems();
            var items = await _workItemRepository.FindAllWorkItems(AzureDevOpsWorkItemService.NAME);
            foreach (var item in items)
            {
                var timeSinceLastSynchronization = DateTime.UtcNow.Subtract(item.LastSynchronizationAtUtc);
                if (timeSinceLastSynchronization.TotalMinutes < 1) continue;
                var incident = await _incidentRepository.GetAsync(item.IncidentId);
                if (incident.State == IncidentState.Closed || incident.State == IncidentState.Ignored) continue;

                var settings = await GetSettings(item.ApplicationId);

                var workItemClient =
                    new WorkItemClient(settings);
                var workItem = await workItemClient.Get(int.Parse(item.WorkItemId));
                if (workItem == null)
                {
                    _logger.Warn(
                        $"Failed to find linked work item for incident {incident.Id}, deleting link for work item {item.WorkItemId}.");
                    await _workItemRepository.Delete(item);
                    continue;
                }

                var workItemState = workItem.Fields[Fields.State].ToString();

                //if (workItemState == settings.AssignedStateName && incident.State == IncidentState.Active)
                //{
                //    continue;
                //}
                if (workItemState == settings.ClosedStateName && incident.State == IncidentState.Closed) continue;
                if (workItemState == settings.SolvedStateName && incident.State == IncidentState.Closed) continue;

                // Don't care about external state for assigned. If it's for a person, then change it.
                if (incident.State == IncidentState.New || incident.State == IncidentState.Active)
                    await AssignIncident(workItem, incident, settings);

                if (workItemState == settings.SolvedStateName || workItemState == settings.ClosedStateName)
                    await CloseIncident(workItem, item, incident);
            }
        }

        private async Task<Settings> GetSettings(int applicationId)
        {
            if (!_settingsCache.TryGetValue(applicationId, out var settings))
            {
                settings = await _settingsRepository.Get(applicationId);
                _settingsCache[applicationId] = settings;
            }

            return settings;
        }

        private async Task AssignIncident(WorkItem workItem, Incident incident, Settings settings)
        {
            var assignedToId = await FindAccountId(workItem, Fields.AssignedTo, Fields.CreatedBy, Fields.ChangedBy,
                Fields.ClosedBy);
            if (assignedToId == null)
                return;


            if (incident.AssignedToId != assignedToId)
            {
                var cmd = new AssignIncident(incident.Id, assignedToId.Value, settings.CreatedById);
                _logger.Info($"Assigning incident {incident.Id} as requested by work item {workItem.Id}.");
                await _messageBus.SendAsync(cmd);
            }
        }

        private async Task CloseIncident(WorkItem workItem, WorkItemMapping item,
            Incident incident)
        {
            var closedById = await FindAccountId(workItem, Fields.ClosedBy, Fields.ResolvedBy, Fields.ChangedBy,
                Fields.AssignedTo);
            if (closedById == null)
                return;


            workItem.Fields.TryGetValue(Fields.ResolvedReason, out var resolvedReason);

            _logger.Info(
                $"Closing incident {item.IncidentId} as requested by work item {item.WorkItemId} (resolved state).");
            var cmd = new CloseIncident(resolvedReason?.ToString() ?? "", incident.Id) { UserId = closedById.Value };
            await _messageBus.SendAsync(cmd);
        }

        private async Task<int?> FindAccountId(WorkItem workItem, params string[] fieldNames)
        {
            var accountRef = GetIdentityRef(workItem, fieldNames);
            var map = await _userMappingRepository.GetByExternalId(accountRef.Id);
            if (map != null)
            {
                return map.AccountId;
            }

            _logger.Warn($"Did not find one of {string.Join(",", fieldNames)} for work item {workItem}.");
            return null;

        }

        private static IdentityRef GetIdentityRef(WorkItem workItem, string[] fieldNames)
        {
            IdentityRef accountRef = null;
            foreach (var fieldName in fieldNames)
            {
                if (!workItem.Fields.TryGetValue(fieldName, out var fieldValue))
                    continue;

                accountRef = (IdentityRef)fieldValue;
                if (accountRef != null)
                    break;
            }

            return accountRef;
        }
    }
}