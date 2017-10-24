using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.ApiKeys.Events
{
    /// <summary>
    /// A new API key has been created.
    /// </summary>
    [Message]
    public class ApiKeyCreated
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ApiKeyCreated" />.
        /// </summary>
        /// <param name="applicationNameForTheAppUsingTheKey">Name of the application that integrates with OTE</param>
        /// <param name="apiKey">Actual key</param>
        /// <param name="sharedSecret">Used to authenticate the key</param>
        /// <param name="applicationIds">Applications that the key is allowed to access</param>
        /// <param name="createdById">User that created this key</param>
        public ApiKeyCreated(string applicationNameForTheAppUsingTheKey, string apiKey, string sharedSecret,
            int[] applicationIds,
            int createdById)
        {
            if (sharedSecret == null) throw new ArgumentNullException("sharedSecret");
            ApplicationIds = applicationIds;
            ApplicationNameForTheAppUsingTheKey = applicationNameForTheAppUsingTheKey;
            ApiKey = apiKey;
            SharedSecret = sharedSecret;
            CreatedById = createdById;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected ApiKeyCreated()
        {
        }

        /// <summary>
        ///     Actual api key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        ///     Applications that the key is allowed to access
        /// </summary>
        public int[] ApplicationIds { get; private set; }

        /// <summary>
        ///     Name of the application that integrates with OTE.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         To allow the user to know which key is used for which integration.
        ///     </para>
        /// </remarks>
        public string ApplicationNameForTheAppUsingTheKey { get; private set; }

        /// <summary>
        ///     Account id of the user that created the key.
        /// </summary>
        public int CreatedById { get; private set; }

        /// <summary>
        ///     Shared secret used to authenticate requests
        /// </summary>
        public string SharedSecret { get; private set; }
    }
}