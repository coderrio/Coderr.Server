using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Api.Core.Incidents.Events;
using Coderr.Server.Domain;
using DotNetCqs;
using log4net;

namespace Coderr.Server.App.WorkItems.Events
{
    internal class CloseWorkItemWhenIncidentClosed : IMessageHandler<IncidentClosed>
    {
        private readonly IWorkItemServiceProvider _itemServiceProvider;
        private readonly IWorkItemRepository _workItemRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(CloseWorkItemWhenIncidentClosed));

        public CloseWorkItemWhenIncidentClosed(IWorkItemServiceProvider itemServiceProvider,
            IWorkItemRepository workItemRepository)
        {
            _itemServiceProvider = itemServiceProvider;
            _workItemRepository = workItemRepository;
        }


        public async Task HandleAsync(IMessageContext context, IncidentClosed message)
        {
            var workItem = await _workItemRepository.Find(message.IncidentId);
            if (workItem == null)
                return;

            var service = await _itemServiceProvider.FindService(workItem.ApplicationId);
            try
            {
                await service.Solve(workItem.ApplicationId, workItem.WorkItemId, message.ClosedById, message.Solution,
                    message.ApplicationVersion);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Error($"Failed to find {workItem.WorkItemId} in azure, invalid mapping for incident {message.IncidentId}.", ex);

                // Been deleted in Azure DevOps
                await _workItemRepository.Delete(workItem);
            }
        }
    }
}