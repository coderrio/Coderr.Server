using System;

namespace codeRR.Server.Api.Core.ApiKeys.Queries
{
    /// <summary>
    ///     Result for <see cref="GetApiKey" />.
    /// </summary>
    public class GetApiKeyResult
    {
        /// <summary>
        ///     Application ids that we've been granted to work with
        /// </summary>
        public GetApiKeyResultApplication[] AllowedApplications { get; set; }

        /// <summary>
        ///     Application that will be using this key
        /// </summary>
        public string ApplicationName { get; set; }


        /// <summary>
        ///     When this key was generated
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     AccountId that generated this key
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        ///     Api key
        /// </summary>
        public string GeneratedKey { get; set; }


        /// <summary>
        ///     PK
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        ///     Used when generating signatures.
        /// </summary>
        public string SharedSecret { get; set; }
    }
}