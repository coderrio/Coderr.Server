namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Reply for <see cref="ResetPassword" />.
    /// </summary>
    public class ResetPasswordReply
    {
        /// <summary>
        ///     Reset was successful.
        /// </summary>
        public bool Success { get; set; }
    }
}