namespace codeRR.Server.Api.Core.Users.Queries
{
    /// <summary>
    ///     Result for <see cref="GetUserSettings" />
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         All settings are system wide except for <see cref="Notifications" />.
    ///     </para>
    /// </remarks>
    public class GetUserSettingsResult
    {
        /// <summary>
        ///     From the user account, always specified.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     First name (optional)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name (optional)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Cell phone number (optional, but required for text notifications).
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        ///     Application specific settings
        /// </summary>
        public NotificationSettings Notifications { get; set; }
    }
}