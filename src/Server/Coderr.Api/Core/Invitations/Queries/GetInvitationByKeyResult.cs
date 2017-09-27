namespace codeRR.Server.Api.Core.Invitations.Queries
{
    /// <summary>
    ///     Result for <see cref="GetInvitationByKey" />.
    /// </summary>
    public class GetInvitationByKeyResult
    {
        /// <summary>
        ///     Email address specified when sending the invitation.
        /// </summary>
        public string EmailAddress { get; set; }
    }
}