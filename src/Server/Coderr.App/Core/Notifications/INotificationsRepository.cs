using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace codeRR.Server.App.Core.Notifications
{
    /// <summary>
    ///     Repository for notification settings
    /// </summary>
    public interface INotificationsRepository
    {
        /// <summary>
        ///     Create settings
        /// </summary>
        /// <param name="notificationSettings">settings</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">notificationSettings</exception>
        Task CreateAsync(UserNotificationSettings notificationSettings);

        /// <summary>
        ///     Check if there are any settings for the given application and the specified user.
        /// </summary>
        /// <param name="accountId">user</param>
        /// <param name="applicationId">application</param>
        /// <returns><c>true</c> if settings exists; otherwise <c>null</c>.</returns>
        Task<bool> ExistsAsync(int accountId, int applicationId);


        /// <summary>
        ///     Get application settings for all users.
        /// </summary>
        /// <param name="applicationId">applicationId</param>
        /// <returns>Default setting will be returned for users that do not have any application specific.</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Task<IEnumerable<UserNotificationSettings>> GetAllAsync(int applicationId);

        /// <summary>
        ///     Get settings
        /// </summary>
        /// <param name="accountId">account that want to modify it's settings</param>
        /// <param name="applicationId">Application to modify settings for (0 = default settings)</param>
        /// <returns>settings if found; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">accountId; applicationId</exception>
        Task<UserNotificationSettings> TryGetAsync(int accountId, int applicationId);

        /// <summary>
        ///     Update notifications
        /// </summary>
        /// <param name="notificationSettings">settings</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">notificationSettings</exception>
        Task UpdateAsync(UserNotificationSettings notificationSettings);
    }
}