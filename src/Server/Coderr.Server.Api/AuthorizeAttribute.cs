using System;
using codeRR.Server.Api.Core;
using codeRR.Server.Api.Core.Accounts.Requests;

namespace codeRR.Server.Api
{
    /// <summary>
    ///     Authorize on specific roles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class), IgnoreField]
    public class AuthorizeRolesAttribute : Attribute
    {
        /// <summary>
        ///     Creates a new instance of <see cref="AuthorizeRolesAttribute" />.
        /// </summary>
        /// <param name="roles">roles granted access</param>
        public AuthorizeRolesAttribute(params string[] roles)
        {
            if (roles == null) throw new ArgumentNullException("roles");
            Roles = roles;
        }

        /// <summary>
        ///     Roles granted access
        /// </summary>
        public string[] Roles { get; private set; }
    }
}