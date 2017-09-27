using DotNetCqs;

namespace codeRR.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Check if the user name or email address are taken
    /// </summary>
    public class ValidateNewLogin : Request<ValidateNewLoginReply>
    {
        /// <summary>
        ///     Email address
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        ///     User name
        /// </summary>
        public string UserName { get; set; }
    }
}