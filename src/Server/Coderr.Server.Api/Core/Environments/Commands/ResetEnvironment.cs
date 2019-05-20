using System;
using Coderr.Server.Api.Core.Environments.Queries;

namespace Coderr.Server.Api.Core.Environments.Commands
{
    /// <summary>
    ///     Delete all incidents in a specific environment
    /// </summary>
    [Command]
    public class ResetEnvironment
    {
        public ResetEnvironment(int applicationId, int environmentId)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));
            if (environmentId <= 0) throw new ArgumentOutOfRangeException(nameof(environmentId));

            ApplicationId = applicationId;
            EnvironmentId = environmentId;
        }

        protected ResetEnvironment()
        {
        }

        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Environment to reset. Id comes from <see cref="GetEnvironments" />.
        /// </summary>
        public int EnvironmentId { get; private set; }
    }
}