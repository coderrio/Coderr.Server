using DotNetCqs;

namespace codeRR.Api.Core.Users.Queries
{
    /// <summary>
    ///     Get settings for an user.
    /// </summary>
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