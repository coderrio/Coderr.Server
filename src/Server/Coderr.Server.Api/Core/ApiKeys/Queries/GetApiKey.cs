using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.ApiKeys.Queries
{
    /// <summary>
    ///     Get information about an API key
    /// </summary>
    [Message]
    public class GetApiKey : Query<GetApiKeyResult>
    {
        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetApiKey()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="GetApiKey" />.
        /// </summary>
        /// <param name="id">PK</param>
        public GetApiKey(int id)
        {
            Id = id;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="GetApiKey" />.
        /// </summary>
        /// <param name="apiKey">The generated ApiKey</param>
        public GetApiKey(string apiKey)
        {
            if (apiKey == null) throw new ArgumentNullException("apiKey");
            Guid guid;
            if (!Guid.TryParse(apiKey, out guid))
                throw new ArgumentException("Not a valid api key: " + apiKey, "apiKey");

            ApiKey = apiKey;
        }

        /// <summary>
        ///     generated api key (if specified)
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        ///     PK (if specified)
        /// </summary>
        public int Id { get; private set; }
    }
}