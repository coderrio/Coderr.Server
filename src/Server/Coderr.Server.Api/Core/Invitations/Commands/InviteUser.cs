using System;
using codeRR.Server.Api.Core.Accounts.Requests;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Invitations.Commands
{
    /// <summary>
    ///     Invite a user to participate in an application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Will send an invitation email to the user if the email is not for a registered user, otherwise we'll just
    ///         add the user as a member of the specified application.
    ///     </para>
    /// </remarks>
    [Message]
    public class InviteUser
    {
        /// <summary>
        ///     Create a new instance of <see cref="InviteUser" />.
        /// </summary>
        /// <param name="applicationId">Application to gain access to</param>
        /// <param name="emailAddress">Email for the given user.</param>
        public InviteUser(int applicationId, string emailAddress)
        {
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
            EmailAddress = emailAddress;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected InviteUser()
        {
        }

        /// <summary>
        ///     Application that the user will get access to.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Email to invited user.
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        ///     A text written by the user to describe why the invite was sent.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     User that invited
        /// </summary>
        [IgnoreField]
        public int UserId { get; set; }
    }
}