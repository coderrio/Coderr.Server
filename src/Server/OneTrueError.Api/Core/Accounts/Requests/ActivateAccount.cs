using System;
using DotNetCqs;

namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Activate a user account
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A user have registered and then clicked on the activation link in the email.
    ///     </para>
    /// </remarks>
    public class ActivateAccount : Request<ActivateAccountReply>
    {
        /// <summary>
        ///     Create a new instance of <see cref="ActivateAccount" />-
        /// </summary>
        /// <param name="activationKey">Activation key from the email.</param>
        public ActivateAccount(string activationKey)
        {
            if (activationKey == null) throw new ArgumentNullException(nameof(activationKey));
            ActivationKey = activationKey;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected ActivateAccount()
        {
        }

        /// <summary>
        ///     Activation key from the email.
        /// </summary>
        public string ActivationKey { get; private set; }
    }
}