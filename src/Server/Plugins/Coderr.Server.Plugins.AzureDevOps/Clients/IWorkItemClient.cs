using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Coderr.Server.Common.AzureDevOps.App.Clients
{
    public interface IWorkItemClient
    {
        /// <summary>
        ///     Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        Task<Comment> AddComment(int workItemId, string comment);

        /// <summary>
        ///     Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        Task<WorkItem> Create(WorkItemDTO workItemDto, string areaPath);

        /// <summary>
        ///     Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        Task<WorkItem> Update(int workItemId, Abstractions.WorkItems.WorkItemMapping workItem);

        Task<WorkItem> Get(int workItemId);
        Task Assign(int workItemId, string emailAddress);
        Task Solve(int workItemId, string emailAddress, string solution, string version);
       
    }
}