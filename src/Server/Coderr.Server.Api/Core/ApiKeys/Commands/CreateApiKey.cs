using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.ApiKeys.Commands
{
    /// <summary>
    ///     Create a new API key
    /// </summary>
    /// <remarks>
    ///     <para>API keys are used to be able to communicate with the codeRR server through the HTTP API.</para>
    /// </remarks>
    [AuthorizeRoles("SysAdmin")]
    [Message]
    public class CreateApiKey
    {
        /// <summary>
        ///     Creates a new instance of <see cref="CreateApiKey" />.
        /// </summary>
        /// <param name="applicationName">
        ///     <see cref="ApplicationName" />
        /// </param>
        /// <param name="apiKey">
        ///     <see cref="ApiKey" />
        /// </param>
        /// <param name="sharedSecret">
        ///     <see cref="SharedSecret" />
        /// </param>
        /// <param name="applicationIds">
        ///     <see cref="ApplicationIds" />
        /// </param>
        /// <exception cref="ArgumentNullException">applicationName;apiKey;sharedSecret;applicationIds</exception>
        public CreateApiKey(string applicationName, string apiKey, string sharedSecret, int[] applicationIds)
        {
            ApplicationName = applicationName ?? throw new ArgumentNullException("applicationName");
            ApiKey = apiKey ?? throw new ArgumentNullException("apiKey");
            SharedSecret = sharedSecret ?? throw new ArgumentNullException("sharedSecret");
            ApplicationIds = applicationIds ?? throw new ArgumentNullException("applicationIds");
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CreateApiKey" />.
        /// </summary>
        /// <param name="applicationName">
        ///     <see cref="ApplicationName" />
        /// </param>
        /// <param name="apiKey">
        ///     <see cref="ApiKey" />
        /// </param>
        /// <param name="sharedSecret">
        ///     <see cref="SharedSecret" />
        /// </param>
        public CreateApiKey(string applicationName, string apiKey, string sharedSecret)
        {
            ApplicationName = applicationName ?? throw new ArgumentNullException("applicationName");
            ApiKey = apiKey ?? throw new ArgumentNullException("apiKey");
            SharedSecret = sharedSecret ?? throw new ArgumentNullException("sharedSecret");
            ApplicationIds = new int[0];
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected CreateApiKey()
        {
        }


        /// <summary>
        ///     Must always be the one that creates the key (will be assigned by the CommandBus per convention)
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        ///     Generated API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        ///     applications that this key may modify. Empty = allow for all applications.
        /// </summary>
        public int[] ApplicationIds { get; set; }

        /// <summary>
        ///     Application that uses this API key
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     Used to sign all requests.
        /// </summary>
        public string SharedSecret { get; set; }
    }
}