using System.Threading.Tasks;

namespace Coderr.Server.Abstractions.WorkItems
{
    public interface IUserMappingRepository
    {
        Task Create(WorkItemUserMapping mapping);
        Task<WorkItemUserMapping> Get(int accountId);
        Task<WorkItemUserMapping> GetByExternalId(string externalId);

    }
}
