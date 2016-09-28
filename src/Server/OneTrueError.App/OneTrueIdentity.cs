using System;
using System.Security.Principal;

namespace OneTrueError.App
{
    /// <summary>
    ///     Identity for <see cref="OneTruePrincipal" />.
    /// </summary>
    public class OneTrueIdentity : IIdentity
    {
        /// <summary>
        ///     Creates a new instance of <see cref="OneTrueIdentity" />.
        /// </summary>
        /// <param name="accountId">Account id</param>
        /// <param name="userName">User name</param>
        public OneTrueIdentity(int accountId, string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            AccountId = accountId;
            Name = userName;
        }

        /// <summary>
        ///     Account id
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        ///     Account name (username)
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     "apiKey", "cookie", "openauth"
        /// </summary>
        public string AuthenticationType { get; set; }

        /// <summary>
        ///     Always true when OneTrueIdentity is used.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return true; }
        }
    }
}