using System;

namespace Coderr.Server.Api.Core.Applications.Commands
{
    [Message]
    public class RenameApplicationGroup
    {
        public RenameApplicationGroup(int groupId, string newName)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId));
            GroupId = groupId;
            NewName = newName ?? throw new ArgumentNullException(nameof(newName));
        }

        public int GroupId { get; private set; }
        public string NewName { get; private set; }
    }
}