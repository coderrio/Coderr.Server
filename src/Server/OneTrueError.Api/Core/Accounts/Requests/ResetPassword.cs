using System;
using DotNetCqs;
using OneTrueError.Api.Core.Accounts.Commands;

namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Reset password (i.e. the second step after <see cref="RequestPasswordReset" />).
    /// </summary>
    public class ResetPassword : Request<ResetPasswordReply>
    {
        /// <summary>
        ///     Create a new instance of <see cref="ResetPassword" />.
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="newPassword"></param>
        public ResetPassword(string activationKey, string newPassword)
        {
            if (activationKey == null) throw new ArgumentNullException("activationKey");
            if (newPassword == null) throw new ArgumentNullException("newPassword");
            ActivationKey = activationKey;
            NewPassword = newPassword;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected ResetPassword()
        {
        }

        /// <summary>
        ///     Activation key, part of the activation email.
        /// </summary>
        public string ActivationKey { get; private set; }

        /// <summary>
        ///     New password as entered by the user.
        /// </summary>
        public string NewPassword { get; private set; }
    }
}