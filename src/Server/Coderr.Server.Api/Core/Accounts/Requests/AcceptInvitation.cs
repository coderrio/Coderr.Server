using System;
using System.ComponentModel.DataAnnotations;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     You must create an account before accepting the invitation
    /// </summary>
    [Command]
    public class AcceptInvitation
    {
        /// <summary>
        ///     Creates a new instance of <see cref="AcceptInvitation" />.
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="password">clear text password</param>
        /// <param name="invitationKey">Key from the generated email.</param>
        public AcceptInvitation(string userName, string password, string invitationKey)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");
            if (invitationKey == null) throw new ArgumentNullException("invitationKey");

            UserName = userName;
            Password = password;
            InvitationKey = invitationKey;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AcceptInvitation" />.
        /// </summary>
        /// <param name="accountId">Existing account</param>
        /// <param name="invitationKey">Key from the generated email.</param>
        /// <remarks>
        ///     <para>
        ///         Invite to an existing account.
        ///     </para>
        /// </remarks>
        public AcceptInvitation(int accountId, string invitationKey)
        {
            if (string.IsNullOrEmpty(invitationKey)) throw new ArgumentNullException("invitationKey");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            AccountId = accountId;
            InvitationKey = invitationKey;
        }


        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected AcceptInvitation()
        {
        }


        /// <summary>
        ///     The email that was used when creating an account.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Do note that this email can be different compared to the one that was used when sending the invitation. Make
        ///         sure that this one is assigned to the created account.
        ///     </para>
        /// </remarks>
        [Required]
        public string AcceptedEmail { get; set; }

        /// <summary>
        ///     Invite to an existing account
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Alternative to the <see cref="UserName" />/<see cref="Password" /> combination
        ///     </para>
        /// </remarks>
        public int AccountId { get; set; }

        /// <summary>
        ///     Email that the inviation was sent to
        /// </summary>
        public string EmailUsedForTheInvitation { get; set; }


        /// <summary>
        ///     First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Invitation key from the invitation email.
        /// </summary>
        public string InvitationKey { get; private set; }

        /// <summary>
        ///     Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Clear text password
        /// </summary>
        /// <seealso cref="UserName" />
        public string Password { get; private set; }

        /// <summary>
        ///     Username as entered by the user
        /// </summary>
        /// <remarks>
        ///     <para>Used together with <see cref="Password" /></para>
        ///     <para>Alternative to <see cref="AccountId" /></para>
        /// </remarks>
        public string UserName { get; private set; }
    }
}