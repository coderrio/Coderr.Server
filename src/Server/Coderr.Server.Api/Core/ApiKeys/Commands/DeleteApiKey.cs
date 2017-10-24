using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.ApiKeys.Commands
{
    /// <summary>
    ///     Delete an API key.
    /// </summary>
    [AuthorizeRoles("SysAdmin")]
    [Message]
    public class DeleteApiKey
    {
        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected DeleteApiKey()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteApiKey" />.
        /// </summary>
        /// <param name="id">PK</param>
        public DeleteApiKey(int id)
        {
            Id = id;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteApiKey" />.
        /// </summary>
        /// <param name="apiKey">The generated ApiKey</param>
        public DeleteApiKey(string apiKey)
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