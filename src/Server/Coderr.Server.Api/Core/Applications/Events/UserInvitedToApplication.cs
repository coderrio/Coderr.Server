using System;
using codeRR.Server.Api.Core.Invitations.Commands;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Events
{
    /// <summary>
    ///     Event published when the <see cref="InviteUser" /> command is done.
    /// </summary>
    [Message]
    public class UserInvitedToApplication
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UserInvitedToApplication" />.
        /// </summary>
        /// <param name="invitationKey">Key that the user clicks on in the invitation email</param>
        /// <param name="applicationId">Application that the user was invited to</param>
        /// <param name="applicationName">application name</param>
        /// <param name="emailAddress">Email address that the invitation was sent to</param>
        /// <param name="invitedBy">Username for the user that made the invitation</param>
        /// <exception cref="ArgumentNullException">emailAddress; invitedBy</exception>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public UserInvitedToApplication(string invitationKey, int applicationId, string applicationName,
            string emailAddress, string invitedBy)
        {
            if (invitationKey == null) throw new ArgumentNullException("invitationKey");
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            if (invitedBy == null) throw new ArgumentNullException("invitedBy");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            InvitationKey = invitationKey;
            ApplicationId = applicationId;
            ApplicationName = applicationName;
            EmailAddress = emailAddress;
            InvitedBy = invitedBy;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected UserInvitedToApplication()
        {
        }

        /// <summary>
        ///     Application that the user will gain access to.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Application name
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        ///     Email address to the invited user.
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        ///     Identifier sent in the invitation email.
        /// </summary>
        public string InvitationKey { get; private set; }

        /// <summary>
        ///     Username of the user that invited the other user.
        /// </summary>
        public string InvitedBy { get; private set; }
    }
}