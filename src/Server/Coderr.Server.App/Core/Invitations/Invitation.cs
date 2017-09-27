using System;
using System.Collections.Generic;

namespace codeRR.Server.App.Core.Invitations
{
    /// <summary>
    ///     Invitation to join this OTE installation
    /// </summary>
    public class Invitation
    {
        /// <summary>
        ///     Creates a new instance of <see cref="Invitation" />.
        /// </summary>
        /// <param name="emailToInvitedUser">Email to the user that is not a member yet</param>
        /// <param name="userNameForInviter">Username for the one that made the invitation.</param>
        /// <exception cref="ArgumentNullException">emailToInvitedUser; userNameForInviter</exception>
        public Invitation(string emailToInvitedUser, string userNameForInviter)
        {
            if (emailToInvitedUser == null) throw new ArgumentNullException("emailToInvitedUser");
            if (userNameForInviter == null) throw new ArgumentNullException("userNameForInviter");

            EmailToInvitedUser = emailToInvitedUser;
            InvitedBy = userNameForInviter;
            InvitationKey = Guid.NewGuid().ToString("N");
            CreatedAtUtc = DateTime.UtcNow;
            Invitations = new List<ApplicationInvitation>();
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected Invitation()
        {
            Invitations = new List<ApplicationInvitation>();
        }

        /// <summary>
        ///     When the invitation request was created
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        /// <summary>
        ///     Email to the user that was invited.
        /// </summary>
        public string EmailToInvitedUser { get; private set; }

        /// <summary>
        ///     Id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Used when clicking on the invitation link to identify this specific invitation
        /// </summary>
        public string InvitationKey { get; private set; }

        /// <summary>
        ///     All applications that the user will get membership to once accepting the invitation.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         A new item is created for every application that the user is invited to.
        ///     </para>
        /// </remarks>
        public IList<ApplicationInvitation> Invitations { get; private set; }

        /// <summary>
        ///     Username of the user that sent the invitation
        /// </summary>
        public string InvitedBy { get; private set; }

        /// <summary>
        ///     Add a mapping for another application (user will gain access to all applications once the invitation is accepted).
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <param name="invitedByUser">user that made this invitation</param>
        /// <exception cref="ArgumentNullException">invitedByUser</exception>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public void Add(int applicationId, string invitedByUser)
        {
            if (invitedByUser == null) throw new ArgumentNullException("invitedByUser");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");

            Invitations.Add(new ApplicationInvitation
            {
                ApplicationId = applicationId,
                InvitedBy = invitedByUser,
                InvitedAtUtc = DateTime.UtcNow
            });
        }
    }
}