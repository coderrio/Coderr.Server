using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace OneTrueError.App
{
    /// <summary>
    ///     Our security principal
    /// </summary>
    public class OneTruePrincipal : IPrincipal
    {
        private readonly string[] _roles;

        /// <summary>
        ///     Creates a new instance of <see cref="OneTruePrincipal" />.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userName">Logged in user or <c>"system"</c></param>
        /// <param name="roles"></param>
        /// <exception cref="ArgumentNullException">userName</exception>
        public OneTruePrincipal(int accountId, string userName, string[] roles)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (roles == null) throw new ArgumentNullException("roles");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            Identity = new OneTrueIdentity(accountId, userName);
            _roles = roles;
        }

        /// <summary>
        ///     Current thread principal
        /// </summary>
        public static OneTruePrincipal Current
        {
            get { return (OneTruePrincipal) Thread.CurrentPrincipal; }
        }

        /// <summary>
        ///     Currently logged in user
        /// </summary>
        public OneTrueIdentity Identity { get; private set; }


        /// <summary>
        ///     Not supported (what a lovely LSP violation)
        /// </summary>
        /// <param name="role">"sysadmin", "admin-x" where X = application id</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">not supported</exception>
        public bool IsInRole(string role)
        {
            if (role == null) throw new ArgumentNullException("role");
            return _roles.Any(x => x.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        IIdentity IPrincipal.Identity
        {
            get { return Identity; }
        }
    }
}