using System.Threading.Tasks;

namespace OneTrueError.App.Modules.Versions.Events
{
    /// <summary>
    ///     Repository
    /// </summary>
    public interface IVersionRepository
    {
        Task CreateAsync(ApplicationVersionMonth month);
        Task CreateAsync(ApplicationVersion entity);
        Task<ApplicationVersionMonth> GetMonthForApplicationAsync(int applicationId, int year, int month);
        Task<ApplicationVersion> GetVersionAsync(int applicationId, string version);
        Task UpdateAsync(ApplicationVersionMonth month);
        Task UpdateAsync(ApplicationVersion entity);
    }
}