using System;

namespace Coderr.Server.App.Core.Environments
{
    /// <summary>
    ///     A mapping between an application and an environment.
    /// </summary>
    public class ApplicationEnvironment
    {
        public ApplicationEnvironment(int applicationId, int environmentId)
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

        protected ApplicationEnvironment()
        {
        }

        /// <summary>
        ///     Mapping for this application
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Do not track incidents in this environment.
        /// </summary>
        public bool DeleteIncidents { get; set; }

        /// <summary>
        ///     Environment that this is (from the Environments table),.
        /// </summary>
        public int EnvironmentId { get; private set; }

        /// <summary>
        ///     Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name as used when reporting errors (from environments table).
        /// </summary>
        public string Name { get; set; }
    }
}