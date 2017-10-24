using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Commands
{
    /// <summary>
    ///     Remove a team member from the
    /// </summary>
    [Message]
    public class RemoveTeamMember
    {
        /// <summary>
        ///     Creates a new instance of <see cref="RemoveTeamMember" />.
        /// </summary>
        /// <param name="applicationId">Application to remove user from</param>
        /// <param name="userToRemove">User id</param>
        public RemoveTeamMember(int applicationId, int userToRemove)
        {
            if (applicationId <= 0) throw new ArgumentOutOfRangeException("applicationId");
            if (userToRemove <= 0) throw new ArgumentOutOfRangeException("userToRemove");
            ApplicationId = applicationId;
            UserToRemove = userToRemove;
        }

        /// <summary>
        ///     application to remove user from
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     User id
        /// </summary>
        public int UserToRemove { get; private set; }
    }
}