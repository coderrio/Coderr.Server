using System;

namespace Coderr.Server.Api.Core.Environments.Commands
{
    /// <summary>
    ///     Create an environment.
    /// </summary>
    [Command]
    public class CreateEnvironment
    {
        public CreateEnvironment(int applicationId, string name)
        {
            if (applicationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(applicationId));
            }

            ApplicationId = applicationId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        protected CreateEnvironment()
        {
        }

        /// <summary>
        ///     Application that the environment belongs to.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Do not track incidents in this environment.
        /// </summary>
        public bool DeleteIncidents { get; set; }

        /// <summary>
        ///     Name used when reporting errors using the client library.
        /// </summary>
        public string Name { get; private set; }
    }
}