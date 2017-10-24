using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Versions.Queries
{
    /// <summary>
    ///     Get all application versions that we've received incidents for
    /// </summary>
    [Message]
    public class GetApplicationVersions : Query<GetApplicationVersionsResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationVersions" />.
        /// </summary>
        /// <param name="applicationId">application to get versions for</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GetApplicationVersions(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Application id
        /// </summary>
        public int ApplicationId { get; private set; }
    }
}