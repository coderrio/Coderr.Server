using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Commands
{
    /// <summary>
    ///     Invited person do not want to accept the invitation
    /// </summary>
    [Message]
    public class DeclineInvitation
    {
        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected DeclineInvitation()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DeclineInvitation" />.
        /// </summary>
        /// <param name="invitationId">invitation key (typically a guid)</param>
        public DeclineInvitation(string invitationId)
        {
            if (string.IsNullOrEmpty(invitationId))
                throw new ArgumentException("Argument is null or empty", "invitationId");
            InvitationId = invitationId;
        }

        /// <summary>
        ///     Invitation key (typically a guid)
        /// </summary>
        public string InvitationId { get; private set; }
    }
}