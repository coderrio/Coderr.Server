using System;
using codeRR.Server.Api.Core.Accounts.Commands;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Change password.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Done when the user knows the current one but want to switch. Otherwise use <see cref="RequestPasswordReset" />.
    ///     </para>
    /// </remarks>
    [Command]
    public class ChangePassword
    {
        /// <summary>
        ///     Create a new instance of <see cref="ChangePassword" />.
        /// </summary>
        /// <param name="currentPassword">Current password</param>
        /// <param name="newPassword">Password to change to.</param>
        public ChangePassword(string currentPassword, string newPassword)
        {
            if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));
            if (newPassword == null) throw new ArgumentNullException(nameof(newPassword));
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected ChangePassword()
        {
        }

        /// <summary>
        ///     Current password
        /// </summary>
        public string CurrentPassword { get; private set; }

        /// <summary>
        ///     Password to change to.
        /// </summary>
        public string NewPassword { get; private set; }

        /// <summary>
        ///     Assigned by the CQS library
        /// </summary>
        [IgnoreField]
        public int UserId { get; set; }
    }
}