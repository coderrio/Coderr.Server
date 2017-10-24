using System;
using DotNetCqs;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace codeRR.Server.Api.Core.Accounts.Commands
{
    /// <summary>
    ///     Register a new account and send out an activation email.
    /// </summary>
    [Message]
    public class RegisterAccount
    {
        /// <summary>
        ///     Creates a new instance of <see cref="RegisterAccount" />
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password as entered by the user</param>
        /// <param name="email">Email address</param>
        public RegisterAccount(string userName, string password, string email)
        {
            UserName = userName ?? throw new ArgumentNullException("userName");
            Password = password ?? throw new ArgumentNullException("password");
            Email = email ?? throw new ArgumentNullException("email");
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected RegisterAccount()
        {
        }

        /// <summary>
        ///     Use a specific account id
        /// </summary>
        /// <remarks>
        ///     <para>0 = auto increment</para>
        /// </remarks>
        public int AccountId { get; private set; }

        /// <summary>
        ///     do not send an activation email, activate the account directly.
        /// </summary>
        public bool ActivateDirectly { get; private set; }

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

        /// <summary>
        ///     Activate this account directly
        /// </summary>
        /// <param name="accountId">Id of the account</param>
        public void Activate(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            ActivateDirectly = true;
            AccountId = accountId;
        }
    }
}