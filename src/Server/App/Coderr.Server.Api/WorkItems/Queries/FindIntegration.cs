using System;
using DotNetCqs;

namespace Coderr.Server.Api.WorkItems.Queries
{
    /// <summary>
    ///     Checks if we have an integration with a work planning site
    /// </summary>
    [Message]
    public class FindIntegration : Query<FindIntegrationResult>
    {
        public FindIntegration(int applicationId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            ApplicationId = applicationId;
        }

        protected FindIntegration()
        {
        }

        /// <summary>
        /// Application to find an integration for.
        /// </summary>
        public int ApplicationId { get; private set; }
    }
}