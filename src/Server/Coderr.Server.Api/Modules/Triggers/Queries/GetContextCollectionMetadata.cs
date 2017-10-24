using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Triggers.Queries
{
    /// <summary>
    ///     Get metadata (context collection information)
    /// </summary>
    [Message]
    public class GetContextCollectionMetadata : Query<GetContextCollectionMetadataItem[]>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetContextCollectionMetadata" />.
        /// </summary>
        /// <param name="applicationId">applicationId</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public GetContextCollectionMetadata(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Application to get info for.
        /// </summary>
        public int ApplicationId { get; private set; }
    }
}