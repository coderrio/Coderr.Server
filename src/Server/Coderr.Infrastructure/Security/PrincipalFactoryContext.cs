using System;
using System.Security.Claims;

namespace codeRR.Server.Infrastructure.Security
{
    /// <summary>
    ///     Context for the factory method in <see cref="PrincipalFactory" />.
    /// </summary>
    public class PrincipalFactoryContext
    {
        /// <summary>
        ///     Creates a new instance of <see cref="PrincipalFactoryContext" />.
        /// </summary>
        /// <param name="accountId">Account, can be 0 = API key</param>
        /// <param name="userName">user/login name</param>
        /// <param name="roles">Roles that the user is a member of</param>
        public PrincipalFactoryContext(int accountId, string userName, string[] roles)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (roles == null) throw new ArgumentNullException("roles");

            AccountId = accountId;
            UserName = userName;
            Roles = roles;
        }

        /// <summary>
        ///     Account from the account table
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        ///     Hint to show how the authentication was made.
        /// </summary>
        public string AuthenticationType { get; set; }

        /// <summary>
        ///     Claims for the user.
        /// </summary>
        public Claim[] Claims { get; set; }

        /// <summary>
        ///     Roles that the user is a member of
        /// </summary>
        public string[] Roles { get; private set; }

        /// <summary>
        ///     User/login name
        /// </summary>
        public string UserName { get; private set; }
    }
}