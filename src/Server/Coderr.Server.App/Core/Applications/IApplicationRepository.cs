using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using codeRR.Server.App.Core.Users;
using Griffin.Data;

namespace codeRR.Server.App.Core.Applications
{
    /// <summary>
    ///     Repository for application management.
    /// </summary>
    public interface IApplicationRepository
    {
        /// <summary>
        ///     Create app.
        /// </summary>
        /// <param name="application">Application to create</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">application</exception>
        Task CreateAsync(Application application);

        /// <summary>
        ///     Create member async
        /// </summary>
        /// <param name="member">member</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">application</exception>
        Task CreateAsync(ApplicationTeamMember member);

        /// <summary>
        ///     Delete application
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task DeleteAsync(int applicationId);

        /// <summary>
        ///     Get all applications
        /// </summary>
        /// <returns>apps</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Task<Application[]> GetAllAsync();

        /// <summary>
        ///     Get application
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <returns>application</returns>
        /// <exception cref="EntityNotFoundException">No application exist with the given application id</exception>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task<Application> GetByIdAsync(int applicationId);

        /// <summary>
        ///     Get by application key
        /// </summary>
        /// <param name="appKey">application key</param>
        /// <returns>application</returns>
        /// <exception cref="ArgumentNullException">appKey</exception>
        /// <exception cref="EntityNotFoundException">No application exist with the given key.</exception>
        Task<Application> GetByKeyAsync(string appKey);

        /// <summary>
        ///     Get all applications that the user is a member of.
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <returns>applications</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        Task<UserApplication[]> GetForUserAsync(int accountId);

        /// <summary>
        ///     Get all members of an application
        /// </summary>
        /// <param name="applicationId">applicationID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IList<ApplicationTeamMember>> GetTeamMembersAsync(int applicationId);

        /// <summary>
        ///     remove a member from an application
        /// </summary>
        /// <param name="applicationId">app</param>
        /// <param name="userId">user</param>
        /// <returns>task</returns>
        Task RemoveTeamMemberAsync(int applicationId, int userId);

        /// <summary>
        ///     Update application member
        /// </summary>
        /// <param name="member">member</param>
        /// <returns>task</returns>
        Task UpdateAsync(ApplicationTeamMember member);

        /// <summary>
        ///     Update application.
        /// </summary>
        /// <param name="entity">app</param>
        /// <returns>task</returns>
        Task UpdateAsync(Application entity);
    }
}