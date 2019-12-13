using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Coderr.Server.Domain.Modules.UserNotifications
{
    public interface IUserNotificationsRepository
    {
        /// <summary>
        ///     Get application settings for all users.
        /// </summary>
        /// <param name="applicationId">applicationId</param>
        /// <returns>Default setting will be returned for users that do not have any application specific.</returns>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Task<IEnumerable<UserNotificationSettings>> GetAllAsync(int applicationId);

        Task<IReadOnlyList<BrowserSubscription>> GetSubscriptions(int accountId);
        Task Delete(BrowserSubscription subscription);

    }
}
