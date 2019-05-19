using Coderr.Server.Api.Core.Environments.Queries;

namespace Coderr.Server.Api.Core.Environments.Commands
{
    /// <summary>
    ///     Delete all incidents in a specific environment
    /// </summary>
    [Command]
    public class ResetEnvironment
    {
        /// <summary>
        /// Environment to reset. Id comes from <see cref="GetEnvironments"/>.
        /// </summary>
        public int EnvironmentId { get; set; }
    }
}