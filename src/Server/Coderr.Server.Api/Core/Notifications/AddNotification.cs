using System;
using DotNetCqs;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace codeRR.Server.Api.Core.Notifications
{
    /// <summary>
    ///     Add a user notification
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         User notifications are typically used when the user need to do some action (typically due to configuration
    ///         issues).
    ///     </para>
    /// </remarks>
    [Message]
    public class AddNotification
    {
        /// <summary>
        ///     Creates a new instance of <see cref="AddNotification" />.
        /// </summary>
        /// <param name="accountId">user account id</param>
        /// <param name="message">message to display</param>
        public AddNotification(int accountId, string message)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            Message = message ?? throw new ArgumentNullException("message");
            AccountId = accountId;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AddNotification" />.
        /// </summary>
        /// <param name="roleName">Send this message to first user that logs in with the specified role</param>
        /// <param name="message">Message to display to the user</param>
        public AddNotification(string roleName, string message)
        {
            RoleName = roleName ?? throw new ArgumentNullException("roleName");
            Message = message ?? throw new ArgumentNullException("message");
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        protected AddNotification()
        {
            
        }

        /// <summary>
        /// Display only for the specified user.
        /// </summary>
        public int? AccountId { get; private set; }


        /// <summary>
        ///     Amount of time to wait until creating this notification again once the user have read the notification.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Requires <see cref="NotificationType" /> to be set.
        ///     </para>
        /// </remarks>
        public TimeSpan? HoldbackInterval { get; set; }

        /// <summary>
        ///     Message to display to user
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///     There may only exist one notification of each type for the target user(s).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Set this to a unique value for your module if you want to prevent multiple instances of the same notification
        ///         to be created. Useful for instance if you sent a configuration failure message when a new report is created.
        ///         Without this type, the same notification would be created every time a report arrives until the configuration
        ///         have been corrected.
        ///     </para>
        ///     <para>
        ///         The notification will be sent again when the user have read it, unless you also have set the hold-back
        ///         timespan.
        ///     </para>
        /// </remarks>
        public string NotificationType { get; set; }

        /// <summary>
        /// Display this message for everyone with the given role
        /// </summary>
        /// <remarks>
        /// <para>
        /// Alternative to <see cref="AccountId"/>.
        /// </para>
        /// </remarks>
        public string RoleName { get; private set; }
    }
}