namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    /// How the login went
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// Account is or became locked
        /// </summary>
        Locked,

        /// <summary>
        /// Incorrect username or password.
        /// </summary>
        IncorrectLogin,

        /// <summary>
        /// Yay!
        /// </summary>
        Successful
    }
}