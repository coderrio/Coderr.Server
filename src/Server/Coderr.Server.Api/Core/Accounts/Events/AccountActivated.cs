using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Events
{
    /// <summary>
    ///     Published when the user have clicked on the activation link in the registration email.
    /// </summary>
    [Message]
    public class AccountActivated
    {
        /// <summary>
        ///     Creates a new instance of <see cref="AccountActivated" />-
        /// </summary>
        /// <param name="accountId">Primary key for the created account</param>
        /// <param name="userName">user name that the account was created with.</param>
        public AccountActivated(int accountId, string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            AccountId = accountId;
            UserName = userName;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected AccountActivated()
        {
        }

        /// <summary>
        ///     Primary key for the account
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        ///     Email address associated with the account.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Unique user name
        /// </summary>
        public string UserName { get; set; }
    }
}