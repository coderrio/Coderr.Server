using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Invitations.Queries
{
    /// <summary>
    ///     Get invitation by using the emailed invitation key
    /// </summary>
    [Message]
    public class GetInvitationByKey : Query<GetInvitationByKeyResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetInvitationByKey" />.
        /// </summary>
        /// <param name="invitationKey">Emailed key</param>
        /// <exception cref="ArgumentNullException">invitationKey</exception>
        public GetInvitationByKey(string invitationKey)
        {
            if (invitationKey == null) throw new ArgumentNullException("invitationKey");
            InvitationKey = invitationKey;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetInvitationByKey()
        {
        }

        /// <summary>
        ///     Invitation key
        /// </summary>
        public string InvitationKey { get; private set; }
    }
}