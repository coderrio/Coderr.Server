using System;
using DotNetCqs;
using OneTrueError.Api.Core.Invitations.Commands;

namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     You must create an account before accepting the invitation
    /// </summary>
    public class AcceptInvitation : Request<AcceptInvitationReply>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AcceptInvitation"/>.
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
        /// Serialization constructor
        /// </summary>
        protected AcceptInvitation()
        {
            
        }

        /// <summary>
        /// Username as entered by the user
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Clear text password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Invitation key from the invitation email.
        /// </summary>
        public string InvitationKey { get; private set; }


        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }


        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }
    }
}