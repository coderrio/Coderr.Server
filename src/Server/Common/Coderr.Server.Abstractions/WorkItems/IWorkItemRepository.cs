using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coderr.Server.Abstractions.WorkItems
{
    public interface IWorkItemRepository
    {
        Task<WorkItemMapping> Find(int incidentId);
        Task<IReadOnlyList<WorkItemMapping>> FindAllWorkItems(string integrationName);
        Task Create(WorkItemMapping item);

        Task MapApplication(int applicationId, string integrationName);

        Task<string> GetIntegrationName(int applicationId);
        Task DeleteAbandonedWorkItems();
        Task Delete(WorkItemMapping workItem);
    }
}
