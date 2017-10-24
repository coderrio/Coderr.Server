using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Events
{
    /// <summary>
    ///     A user have accepted an invitation.
    /// </summary>
    [Message]
    public class InvitationAccepted
    {
        /// <summary>
        ///     Creates a new instance of <see cref="InvitationAccepted" />.
        /// </summary>
        /// <param name="accountId">account that accepted the invitation</param>
        /// <param name="invitedByUserName">user that made the invite</param>
        /// <param name="userName">userName of the person that accepted the invitation</param>
        /// <exception cref="ArgumentNullException">invitedByUserName; userName</exception>
        /// <exception cref="ArgumentOutOfRangeException">accountId</exception>
        public InvitationAccepted(int accountId, string invitedByUserName, string userName)
        {
            if (invitedByUserName == null) throw new ArgumentNullException("invitedByUserName");
            if (userName == null) throw new ArgumentNullException("userName");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            AccountId = accountId;
            InvitedByUserName = invitedByUserName;
            UserName = userName;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected InvitationAccepted()
        {
        }

        /// <summary>
        ///     The email that the invitation was accepted by.
        /// </summary>
        public string AcceptedEmailAddress { get; set; }

        /// <summary>
        ///     Id of the user that accepted the invitation
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        ///     Applications that the user got access to.
        /// </summary>
        public int[] ApplicationIds { get; set; }

        /// <summary>
        ///     User that created the invite.
        /// </summary>
        public string InvitedByUserName { get; set; }

        /// <summary>
        ///     Email address that the invitation was sent to.
        /// </summary>
        public string InvitedEmailAddress { get; set; }

        /// <summary>
        ///     The user that accepted the invitation
        /// </summary>
        public string UserName { get; set; }
    }
}