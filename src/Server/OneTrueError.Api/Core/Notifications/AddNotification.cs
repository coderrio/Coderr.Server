using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCqs;

namespace OneTrueError.Api.Core.Notifications
{
    public class AddNotification : Command
    {
        public AddNotification(int accountId, string message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            AccountId = accountId;
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AddNotification"/>.
        /// </summary>
        /// <param name="roleName">Send this message to first user that logs in with the specified role</param>
        /// <param name="message"></param>
        public AddNotification(string roleName, string message)
        {
            if (roleName == null) throw new ArgumentNullException("roleName");
            if (message == null) throw new ArgumentNullException("message");
            RoleName = roleName;
            Message = message;
        }

        /// <summary>
        /// There may only exist one notification of each type for the target user(s).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set this to a unique value for your module if you want to prevent multiple instances of the same notification to be created. Useful for instance if you sent a configuration failure message when a new report is created. Without this type, the same notification would be created every time a report arrives until the configuration have been corrected.
        /// </para>
        /// <para>
        /// The notification will be sent again when the user have read it, unless you also have set the hold-back timespan.
        /// </para>
        /// </remarks>
        public string NotificationType { get; set; }


        /// <summary>
        /// Amount of time to wait until creating this notification again once the user have read the notification.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Requires <see cref="NotificationType"/> to be set.
        /// </para>
        /// </remarks>
        public TimeSpan? HoldbackInterval { get; set; }
        /// <summary>
        /// Message to display to user
        /// </summary>
        public string Message { get; private set; }

        public int? AccountId { get; private set; }

        public string RoleName { get; private set; }
    }
}
