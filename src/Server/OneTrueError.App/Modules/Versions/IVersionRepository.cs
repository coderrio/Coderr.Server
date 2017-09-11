using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace OneTrueError.App.Modules.Versions
{
    /// <summary>
    ///     Repository definition to be able to persist information about application versions
    /// </summary>
    public interface IVersionRepository
    {
        /// <summary>
        ///     Create statistics information for version usage.
        /// </summary>
        /// <param name="month">info</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">month</exception>
        /// <exception cref="DbException">Failed to insert row</exception>
        Task CreateAsync(ApplicationVersionMonth month);

        /// <summary>
        ///     Create a new application version
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">month</exception>
        /// <exception cref="DbException">Failed to insert row</exception>
        Task CreateAsync(ApplicationVersion entity);

        /// <summary>
        ///     Get monhtly exception report
        /// </summary>
        /// <param name="applicationId">Application id</param>
        /// <param name="year">Year, four digits</param>
        /// <param name="month">Month</param>
        /// <returns>entity if found; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">applicationId</exception>
        /// <exception cref="ArgumentOutOfRangeException">year;month</exception>
        /// <exception cref="DbException">Failed to query DB</exception>
        Task<ApplicationVersionMonth> FindMonthForApplicationAsync(int applicationId, int year, int month);

        /// <summary>
        ///     Hämta version
        /// </summary>
        /// <param name="applicationId">id for the application that we want to fetch a version for</param>
        /// <param name="version">Version (<c>"1.0.0"</c>)</param>
        /// <returns>version if found; otherwise <c>null</c></returns>
        /// <exception cref="ArgumentNullException">applicationId;version</exception>
        /// <exception cref="DbException">Failed to query DB</exception>
        Task<ApplicationVersion> FindVersionAsync(int applicationId, string version);

        /// <summary>
        ///     Update an existing monthly report
        /// </summary>
        /// <param name="month">entity</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">month</exception>
        /// <exception cref="DbException">Failed to execute sql</exception>
        Task UpdateAsync(ApplicationVersionMonth month);

        /// <summary>
        ///     Update an existing version info
        /// </summary>
        /// <param name="entity">version info</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">enitty</exception>
        /// <exception cref="DbException">Failed to query DB</exception>
        Task UpdateAsync(ApplicationVersion entity);
    }
}