namespace codeRR.Server.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     DTO
    /// </summary>
    public class ValidateNewLoginReply
    {
        /// <summary>
        ///     The given email address is already associated with an account.
        /// </summary>
        public bool EmailIsTaken { get; set; }

        /// <summary>
        ///     The given user name is already associated with an account.
        /// </summary>
        public bool UserNameIsTaken { get; set; }
    }
}