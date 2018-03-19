using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Coderr.Server.Domain.Modules.ApplicationVersions
{
    public interface IApplicationVersionRepository
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
        ///     Get monthly exception report
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
        ///     Get version
        /// </summary>
        /// <param name="applicationId">id for the application that we want to fetch a version for</param>
        /// <param name="version">Version (<c>"1.0.0"</c>)</param>
        /// <returns>version if found; otherwise <c>null</c></returns>
        /// <exception cref="ArgumentNullException">applicationId;version</exception>
        /// <exception cref="DbException">Failed to query DB</exception>
        Task<ApplicationVersion> FindVersionAsync(int applicationId, string version);

        /// <summary>
        ///     Save version (ignore if it have already been stored).
        /// </summary>
        /// <param name="incidentId">incident to attach version to</param>
        /// <param name="versionId">Id for the application version</param>
        void SaveIncidentVersion(int incidentId, int versionId);


        /// <summary>
        ///     Update an existing monthly report
        /// </summary>
        /// <param name="month">entity</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">month</exception>
        /// <exception cref="DbException">Failed to execute SQL</exception>
        Task UpdateAsync(ApplicationVersionMonth month);

        /// <summary>
        ///     Update an existing version info
        /// </summary>
        /// <param name="entity">version info</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        /// <exception cref="DbException">Failed to query DB</exception>
        Task UpdateAsync(ApplicationVersion entity);
    }
}