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
    public class DeleteApplicationGroup
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="DeleteApplicationGroup" /> class.
        /// </summary>
        /// <param name="groupId">Group to delete</param>
        /// <param name="moveAppsToGroupId">Move all applications to this group</param>
        public DeleteApplicationGroup(int groupId, int moveAppsToGroupId)
        {
            GroupId = groupId;
            MoveAppsToGroupId = moveAppsToGroupId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        private DeleteApplicationGroup()
        {
        }

        /// <summary>
        ///     Group to delete
        /// </summary>
        public int GroupId { get; private set; }

        /// <summary>
        /// Move all applications to this group
        /// </summary>
        public int MoveAppsToGroupId { get; private set; }

    }
}