using System;

namespace Coderr.Server.Api.Core.Applications.Commands
{
    /// <summary>
    ///     Create an application group.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Groups are used to group similar applications together.
    ///     </para>
    /// </remarks>
    [Command]
    public class CreateApplicationGroup
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CreateApplicationGroup" /> class.
        /// </summary>
        /// <param name="name">Name of the new group (human friendly name)</param>
        public CreateApplicationGroup(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        private CreateApplicationGroup()
        {
        }

        /// <summary>
        ///     Name of the new group (human friendly name).
        /// </summary>
        public string Name { get; private set; }
    }
}