using DotNetCqs;

namespace codeRR.Server.Api.Core.Support
{
    /// <summary>
    ///     Send a support request to 1TCompany AB
    /// </summary>
    [Message]
    public class SendSupportRequest
    {
        /// <summary>
        ///     Problem statement
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Why do we want support, huh?
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     Url of the page that did not work
        /// </summary>
        public string Url { get; set; }
    }
}