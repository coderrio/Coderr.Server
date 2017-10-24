using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Get stats etc that can be presented as an overview for an application.
    /// </summary>
    [Message]
    public class GetApplicationOverview : Query<GetApplicationOverviewResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationOverview" />.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <exception cref="ArgumentOutOfRangeException">applicationId</exception>
        public GetApplicationOverview(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            ApplicationId = applicationId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetApplicationOverview()
        {
        }

        /// <summary>
        ///     Application id to get an overview for.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Amount of time to look back (i.e. startdate = DateTime.Now.Substract(WindowSize))
        /// </summary>
        /// <remarks>
        ///     1 = switch to hours
        /// </remarks>
        public int NumberOfDays { get; set; }

        /// <summary>
        /// Filter on a specific version ("1.1.0")
        /// </summary>
        public string Version { get; set; }
    }
}