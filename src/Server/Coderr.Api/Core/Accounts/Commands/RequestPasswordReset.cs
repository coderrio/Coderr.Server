using System;
using DotNetCqs;
using codeRR.Api.Core.Accounts.Requests;

namespace codeRR.Api.Core.Accounts.Commands
{
    /// <summary>
    ///     Request a password reset (i.e. lock account, email an activation link to the user and wait for activation).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ResetPassword" /> will be exeucted when the user clicks on the link.
    ///     </para>
    /// </remarks>
    public class RequestPasswordReset : Command
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