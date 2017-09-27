using System;
using codeRR.Server.App.Core.Accounts;

namespace codeRR.Server.App.Core.Users
{
    /// <summary>
    ///     User information associated to an <see cref="Account" />.
    /// </summary>
    public class User
    {
        /// <summary>
        ///     Creates a new instance of <see cref="User" />.
        /// </summary>
        /// <param name="accountId">account id</param>
        /// <param name="userName">username from account object</param>
        /// <exception cref="ArgumentOutOfRangeException">accountId</exception>
        /// <exception cref="ArgumentNullException">userName</exception>
        public User(int accountId, string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            AccountId = accountId;
            UserName = userName;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected User()
        {
        }

        /// <summary>
        ///     Account id
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        ///     Email address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     First name (if specified)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name (if specified)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Mobile number (if specified)
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        ///     userName (same as in the account object)
        /// </summary>
        public string UserName { get; private set; }
    }
}