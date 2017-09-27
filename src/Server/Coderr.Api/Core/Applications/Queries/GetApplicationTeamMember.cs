using System;

namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Item for <see cref="GetApplicationTeamResult" />.
    /// </summary>
    public class GetApplicationTeamMember
    {
        /// <summary>
        ///     When this person was added to the application (or rather when he accepted the invitation)
        /// </summary>
        public DateTime JoinedAtUtc { get; set; }

        /// <summary>
        ///     Account id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Account name
        /// </summary>
        public string UserName { get; set; }
    }
}