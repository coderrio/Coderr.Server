namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Result for <see cref="GetApplicationTeam" />.
    /// </summary>
    public class GetApplicationTeamResult
    {
        /// <summary>
        ///     Invited which have not yet accepted the invitation.
        /// </summary>
        public GetApplicationTeamResultInvitation[] Invited { get; set; }

        /// <summary>
        ///     Members
        /// </summary>
        public GetApplicationTeamMember[] Members { get; set; }
    }
}