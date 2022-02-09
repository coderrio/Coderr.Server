using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coderr.Server.App.Core.Environments
{
    public interface IEnvironmentRepository
    {
        Task Create(Environment environment);
        Task Create(ApplicationEnvironment environment);
        Task Delete(Environment environment);
        Task<ApplicationEnvironment> Find(int environmentId, int applicationId);
        Task<Environment> Find(int environmentId);
        Task<Environment> FindByName(string name);
        Task<IReadOnlyList<Environment>> ListAll();
        Task<IReadOnlyList<ApplicationEnvironment>> ListForApplication(int applicationId);
        Task Reset(int environmentId, int? applicationId);
        Task Update(ApplicationEnvironment environment);
    }
}