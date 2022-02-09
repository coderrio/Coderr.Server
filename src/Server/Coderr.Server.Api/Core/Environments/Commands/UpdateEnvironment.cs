using System;
using Coderr.Server.Api.Core.Environments.Queries;

namespace Coderr.Server.Api.Core.Environments.Commands
{
    /// <summary>
    ///     Update an environment.
    /// </summary>
    [Command]
    public class UpdateEnvironment
    {
        public UpdateEnvironment(int applicationId, int environmentId)
        {
            if (applicationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(applicationId));
            }

            if (environmentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(environmentId));
            }

            ApplicationId = applicationId;
            EnvironmentId = environmentId;
        }

        protected UpdateEnvironment()
        {
        }

        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Do not track incidents in this environment.
        /// </summary>
        public bool DeleteIncidents { get; set; }

        /// <summary>
        ///     Environment to reset. Id comes from <see cref="GetEnvironments" />.
        /// </summary>
        public int EnvironmentId { get; private set; }
    }
}