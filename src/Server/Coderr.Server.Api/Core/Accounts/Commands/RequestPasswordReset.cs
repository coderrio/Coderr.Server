using System;
using codeRR.Server.Api.Core.Accounts.Requests;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Commands
{
    /// <summary>
    ///     Request a password reset (i.e. lock account, email an activation link to the user and wait for activation).
    /// </summary>
    [Message]
    public class RequestPasswordReset
    {
        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected RequestPasswordReset()
        {
        }

        /// <summary>
        ///     Create a new instance of <see cref="RequestPasswordReset" />.
        /// </summary>
        /// <param name="emailAddress">Email address associated with the user account.</param>
        public RequestPasswordReset(string emailAddress)
        {
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            EmailAddress = emailAddress;
        }

        /// <summary>
        ///     Email address associated with the user account.
        /// </summary>
        public string EmailAddress { get; private set; }
    }
}