namespace Coderr.Server.Api.Core.Applications.Commands
{
    /// <summary>
    ///     Assign an application to a group.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Groups are used to make it easier to navigate between applications.
    ///     </para>
    /// </remarks>
    [Command]
    public class SetApplicationGroup
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="SetApplicationGroup" /> class.
        /// </summary>
        /// <param name="applicationId">Application to assign a group to.</param>
        /// <param name="applicationGroupId">Group to assign the application to</param>
        public SetApplicationGroup(int applicationId, int applicationGroupId)
        {
            ApplicationId = applicationId;
            ApplicationGroupId = applicationGroupId;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="SetApplicationGroup" /> class.
        /// </summary>
        /// <param name="applicationId">Application to assign a group to.</param>
        /// <param name="groupName">Group to assign the application to</param>
        public SetApplicationGroup(int applicationId, string groupName)
        {
            ApplicationId = applicationId;
            GroupName = groupName;
        }

        protected SetApplicationGroup()
        {

        }

        /// <summary>
        ///     Group to show the application under.
        /// </summary>
        public int ApplicationGroupId { get; private set; }

        /// <summary>
        ///     Application which should appear under the group
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        /// Group name (you can specify either the group Id or the group name)
        /// </summary>
        public string GroupName { get; private set; }
    }
}