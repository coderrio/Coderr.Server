using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Events
{
    /// <summary>
    ///     A login attempt failed
    /// </summary>
    [Message]
    public class LoginFailed
    {
        /// <summary>
        ///     Creates a new instance of <see cref="LoginFailed" />.
        /// </summary>
        /// <param name="userName">user that attempted to login (userName was entered by the user, it might not exist)</param>
        /// <exception cref="ArgumentNullException">userName</exception>
        public LoginFailed(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            UserName = userName;
        }


        /// <summary>
        ///     If failed login was the reason (can't be set at the same time as <see cref="IsLocked" />)
        /// </summary>
        public bool InvalidLogin { get; set; }

        /// <summary>
        ///     If account have been activated after registration.
        /// </summary>
        public bool IsActivated { get; set; }

        /// <summary>
        ///     If account was or became locked.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        ///     user that attempted to login (userName was entered by the user, it might not exist)
        /// </summary>
        public string UserName { get; private set; }
    }
}