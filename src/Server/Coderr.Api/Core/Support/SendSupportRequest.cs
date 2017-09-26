using DotNetCqs;

namespace OneTrueError.Api.Core.Support
{
    /// <summary>
    ///     Send a support request to Gauffin Interactive AB
    /// </summary>
    public class SendSupportRequest : Command
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