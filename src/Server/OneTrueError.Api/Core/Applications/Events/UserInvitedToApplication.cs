using System;
using DotNetCqs;
using OneTrueError.Api.Core.Invitations.Commands;

namespace OneTrueError.Api.Core.Applications.Events
{
    /// <summary>
    /// Event published when the <see cref="InviteUser"/> command is done.
    /// </summary>
    public class UserInvitedToApplication : ApplicationEvent
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserInvitedToApplication"/>.
        /// </summary>
        /// <param name="applicationId">Application that the user was invited to</param>
        /// <param name="emailAddress">Email address that the invitation was sent to</param>
        /// <param name="invitedBy">Username for the user that made the invitation</param>
        /// <exception cref="ArgumentNullException">emailAddress; invitedBy</exception>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public UserInvitedToApplication(int applicationId, string emailAddress, string invitedBy)
        {
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            if (invitedBy == null) throw new ArgumentNullException("invitedBy");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
            EmailAddress = emailAddress;
            InvitedBy = invitedBy;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        protected UserInvitedToApplication()
        {

        }

        /// <summary>
        /// Application that the user will gain access to.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        /// Email address to the invited user.
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        /// Username of the user that invited the other user.
        /// </summary>
        public string InvitedBy { get; private set; }

    }
}