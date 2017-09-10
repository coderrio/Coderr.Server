using DotNetCqs;

namespace OneTrueError.Api.Core.Users.Commands
{
    /// <summary>
    ///     Update personal settings.
    /// </summary>
    public class UpdatePersonalSettings : Command
    {
        /// <summary>
        ///     First name (if specified)
        /// </summary>
        public string FirstName { get; set; }


        /// <summary>
        ///     Last name (if specified)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Mobile number (E.164 formatted)
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        ///     Account that the settings are for
        /// </summary>
        [IgnoreField]
        public int UserId { get; set; }
    }
}