using System;

namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    /// Reply for <see cref="ActivateAccount"/>.
    /// </summary>
    public class ActivateAccountReply
    {
        /// <summary>
        /// Creates a new instance of <see cref="ActivateAccountReply"/>.
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="userName">Username used when registering the account</param>
        /// <exception cref="ArgumentNullException">userName</exception>
        /// <exception cref="ArgumentOutOfRangeException">accountId</exception>
        public ActivateAccountReply(int accountId, string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            AccountId = accountId;
            UserName = userName;
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected ActivateAccountReply()
        {
            
        }

        /// <summary>
        /// Username as entered when registering.
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// Account identifier.
        /// </summary>
        public int AccountId { get; set; }
    }
}