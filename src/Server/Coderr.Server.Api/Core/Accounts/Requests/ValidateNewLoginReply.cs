namespace codeRR.Server.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Reply for <see cref="ValidateNewLogin" />.
    /// </summary>
    public class ValidateNewLoginReply
    {
        /// <summary>
        ///     The given email address is already associated with an account.
        /// </summary>
        public bool EmailIsTaken { get; set; }

        /// <summary>
        ///     The given username is already associated with an account.
        /// </summary>
        public bool UserNameIsTaken { get; set; }
    }
}