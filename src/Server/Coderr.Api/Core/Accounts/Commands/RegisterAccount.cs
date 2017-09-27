using System;
using DotNetCqs;

namespace codeRR.Api.Core.Accounts.Commands
{
    /// <summary>
    ///     Register a new account and send out an activation email.
    /// </summary>
    public class RegisterAccount : Command
    {
        /// <summary>
        ///     Creates a new instance of <see cref="RegisterAccount" />
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password as entered by the user</param>
        /// <param name="email">Email address</param>
        public RegisterAccount(string userName, string password, string email)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");
            if (email == null) throw new ArgumentNullException("email");
            UserName = userName;
            Password = password;
            Email = email;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected RegisterAccount()
        {
        }

        /// <summary>
        ///     Email address.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        ///     Password as entered by the user.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        ///     User name
        /// </summary>
        public string UserName { get; private set; }
    }
}