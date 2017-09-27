using System;

namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Item for <see cref="GetApplicationTeamResult" />.
    /// </summary>
    public class GetApplicationTeamResultInvitation
    {
        /// <summary>
        ///     Address that the invitation was sent to.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     When the invitation was sent.
        /// </summary>
        public DateTime InvitedAtUtc { get; set; }

        /// <summary>
        ///     User that sent the invitation.
        /// </summary>
        public string InvitedByUserName { get; set; }
    }
}