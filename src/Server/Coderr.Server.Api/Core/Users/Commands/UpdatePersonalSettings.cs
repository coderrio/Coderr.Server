using codeRR.Server.Api.Core.Accounts.Requests;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Users.Commands
{
    /// <summary>
    ///     Update personal settings.
    /// </summary>
    [Message]
    public class UpdatePersonalSettings
    {
        /// <summary>
        ///     Change email address
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Do not required additional verification, we trust the user once it has an activated account.
        ///     </para>
        /// </remarks>
        public string EmailAddress { get; set; }

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