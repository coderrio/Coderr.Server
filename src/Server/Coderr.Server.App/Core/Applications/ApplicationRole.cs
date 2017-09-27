namespace codeRR.Server.App.Core.Applications
{
    /// <summary>
    ///     Roles for <see cref="Application" />.
    /// </summary>
    public static class ApplicationRole
    {
        /// <summary>
        ///     Can change configuration information like members and triggers.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        ///     Can handle incidents (ignore, close etc).
        /// </summary>
        public const string Member = "Member";
    }
}