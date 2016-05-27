using System;
using System.Security.Principal;
using System.Threading;

namespace OneTrueError.App
{
    /// <summary>
    /// Our security principal
    /// </summary>
    public class OneTruePrincipal : IPrincipal
    {
        /// <summary>
        /// Creates a new instance of <see cref="OneTruePrincipal"/>.
        /// </summary>
        /// <param name="userName">Logged in user or <c>"system"</c></param>
        /// <exception cref="ArgumentNullException">userName</exception>
        public OneTruePrincipal(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            Identity = new GenericIdentity(userName);
        }

        /// <summary>
        /// Current thread principal
        /// </summary>
        public static OneTruePrincipal Current
        {
            get { return ((OneTruePrincipal) Thread.CurrentPrincipal); }
        }

        
        /// <summary>
        /// Not supported (what a lovely LSP violation)
        /// </summary>
        /// <param name="role">none</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">not supported</exception>
        public bool IsInRole(string role)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Currently logged in user
        /// </summary>
        public IIdentity Identity { get; private set; }
    }
}