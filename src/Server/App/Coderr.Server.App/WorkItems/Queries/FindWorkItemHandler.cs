using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Api.WorkItems.Queries;
using DotNetCqs;

namespace Coderr.Server.App.WorkItems.Queries
{
    public class FindWorkItemHandler : IQueryHandler<FindWorkItem, FindWorkItemResult>
    {
        private readonly IWorkItemRepository _workItemRepository;

        public FindWorkItemHandler(IWorkItemRepository workItemRepository)
        {
            _workItemRepository = workItemRepository;
        }

        public async Task<FindWorkItemResult> HandleAsync(IMessageContext context, FindWorkItem query)
        {
            var item = await _workItemRepository.Find(query.IncidentId);
            if (item == null)
            {
                return null;
            }

            return new FindWorkItemResult {Name = item.Name, Url = item.UiUrl, WorkItemId = item.WorkItemId, ApplicationId = item.ApplicationId};
        }
    }
}
