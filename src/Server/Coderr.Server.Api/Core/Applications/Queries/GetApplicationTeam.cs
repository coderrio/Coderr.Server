using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Get all members of a specific application
    /// </summary>
    [Message]
    public class GetApplicationTeam : Query<GetApplicationTeamResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationTeam" />.
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public GetApplicationTeam(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetApplicationTeam()
        {
        }

        /// <summary>
        ///     Application id
        /// </summary>
        public int ApplicationId { get; private set; }
    }
}