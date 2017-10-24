using codeRR.Server.Api.Core.Accounts.Requests;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Users.Queries
{
    /// <summary>
    ///     Get settings for an user.
    /// </summary>
    [Message]
    public class GetUserSettings : Query<GetUserSettingsResult>
    {
        /// <summary>
        ///     Get user settings for this application only
        /// </summary>
        public int ApplicationId { get; set; }


        /// <summary>
        ///     User to get settings for
        /// </summary>
        [IgnoreField]
        public int UserId { get; set; }
    }
}