using System.Threading.Tasks;

namespace Coderr.Server.Abstractions.WorkItems
{
    /// <summary>
    /// Used to create and manage work items in an external system.
    /// </summary>
    public interface IWorkItemService
    {
        string Name { get; }
        string Title { get; }
        Task Create(WorkItemDTO workItem);

        /// <summary>
        /// Assign a work item to a person
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="workItemId"></param>
        /// <param name="accountId">Person that got it assigned</param>
        /// <param name="workitemId">Work item to assign</param>
        Task Assign(int applicationId, string workItemId, int accountId);


        /// <summary>
        /// Assign a work item to a person
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="workItemId"></param>
        /// <param name="accountId"></param>
        /// <param name="solution"></param>
        /// <param name="version"></param>
        /// <param name="workitemId"></param>
        Task Solve(int applicationId, string workItemId, int accountId, string solution, string version);

    }
}
