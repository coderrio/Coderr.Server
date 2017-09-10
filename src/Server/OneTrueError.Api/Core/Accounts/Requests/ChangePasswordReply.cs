namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Reply for <see cref="ChangePassword" />.
    /// </summary>
    public class ChangePasswordReply
    {
        /// <summary>
        ///     Change was successful.
        /// </summary>
        public bool Success { get; set; }
    }
}