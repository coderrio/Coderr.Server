using System;
using DotNetCqs;

namespace OneTrueError.Api.Core.ApiKeys.Commands
{
    /// <summary>
    /// Create a new api key
    /// </summary>
    [Authorize("SysAdmin")]
    public class CreateApiKey : Command
    {
        /// <summary>
        /// Creates a new instance of <see cref="CreateApiKey"/>.
        /// </summary>
        /// <param name="applicationName"><see cref="ApplicationName"/></param>
        /// <param name="apiKey"><see cref="ApiKey"/></param>
        /// <param name="sharedSecret"><see cref="SharedSecret"/></param>
        /// <param name="applicationIds"><see cref="ApplicationIds"/></param>
        public CreateApiKey(string applicationName, string apiKey, string sharedSecret, int[] applicationIds)
        {
            if (applicationName == null) throw new ArgumentNullException("applicationName");
            if (apiKey == null) throw new ArgumentNullException("apiKey");
            if (sharedSecret == null) throw new ArgumentNullException("sharedSecret");
            if (applicationIds == null) throw new ArgumentNullException("applicationIds");

            ApplicationName = applicationName;
            ApiKey = apiKey;
            SharedSecret = sharedSecret;
            ApplicationIds = applicationIds;
        }


        /// <summary>
        /// Must always be the one that creates the key (will be assigned by the CommandBus per convention)
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Generated api key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// applications that this key may modify. Empty = allow for all applications.
        /// </summary>
        public int[] ApplicationIds { get; set; }

        /// <summary>
        /// Application that uses this api key
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Used to sign all requests.
        /// </summary>
        public string SharedSecret { get; set; }
    }
}