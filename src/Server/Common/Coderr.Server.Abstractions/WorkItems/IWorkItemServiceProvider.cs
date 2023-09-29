using System.Threading.Tasks;

namespace Coderr.Server.Abstractions.WorkItems
{
    /// <summary>
    /// Used to register services for work items (each integration have its own service).
    /// </summary>
    public interface IWorkItemServiceProvider
    {
        Task CreateAsync(int applicationId, int incidentId);

        Task MapApplication(int applicationId, string integrationName);

        Task<IWorkItemService> FindService(int applicationId);
    }
}
