using System;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace codeRR.Server.Api.Core.Accounts.Events
{
    /// <summary>
    ///     An user have registered an account and activated it.
    /// </summary>
    [Message]
    public class AccountRegistered
    {
        /// <summary>
        ///     Create a new instance of <see cref="AccountRegistered" />-
        /// </summary>
        /// <param name="accountId">Account id (primary key).</param>
        /// <param name="userName">User name as entered by the user.</param>
        public AccountRegistered(int accountId, string userName)
        {
            if (accountId <= 0) throw new ArgumentNullException("accountId");
            AccountId = accountId;
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected AccountRegistered()
        {
        }

        /// <summary>
        ///     Account id (primary key).
        /// </summary>
        
        public int AccountId { get; private set; }

        /// <summary>
        ///     The registered user is a system administrator
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         System administrators can create new applications, decide who is application administrator
        ///         and configure other system wide settings.
        ///     </para>
        /// </remarks>
        public bool IsSysAdmin { get; set; }

        /// <summary>
        ///     User name as entered by the user.
        /// </summary>
        public string UserName { get; private set; }
    }
}