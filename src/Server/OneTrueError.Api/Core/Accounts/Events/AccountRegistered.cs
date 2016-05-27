using System;
using DotNetCqs;

namespace OneTrueError.Api.Core.Accounts.Events
{
    /// <summary>
    ///     An user have registered an account and activated it.
    /// </summary>
    public class AccountRegistered : ApplicationEvent
    {
        /// <summary>
        /// Create a new instance of <see cref="AccountRegistered"/>-
        /// </summary>
        /// <param name="accountId">Account id (primary key).</param>
        /// <param name="userName">User name as entered by the user.</param>
        public AccountRegistered(int accountId, string userName)
        {
            if (accountId <= 0) throw new ArgumentNullException("accountId");
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            AccountId = accountId;
            UserName = userName;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        protected AccountRegistered()
        {
            
        }

        /// <summary>
        /// Account id (primary key).
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// User name as entered by the user.
        /// </summary>
        public string UserName { get; private set; }
    }
}