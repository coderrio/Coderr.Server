using System;

namespace Coderr.Server.Api.Core.Applications.Commands
{
    [Message]
    public class MapApplicationsToGroup
    {
        public MapApplicationsToGroup(int groupId, int[] applicationIds)
        {
            if (applicationIds == null) throw new ArgumentNullException(nameof(applicationIds));
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId));
            GroupId = groupId;
            ApplicationIds = applicationIds ?? throw new ArgumentNullException(nameof(applicationIds));
        }

        protected MapApplicationsToGroup()
        {

        }

        /// <summary>
        /// Applications to assign to the group.
        /// </summary>
        public int[] ApplicationIds { get; private set; }

        /// <summary>
        /// Group that the applications should be assigned to.
        /// </summary>
        public int GroupId { get; private set; }
    }
}
