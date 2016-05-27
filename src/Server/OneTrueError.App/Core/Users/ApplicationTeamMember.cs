using System;
using System.Diagnostics.CodeAnalysis;

namespace OneTrueError.App.Core.Users
{
    /// <summary>
    ///     Used to control access to a specific application.
    /// </summary>
    public class ApplicationTeamMember
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ApplicationTeamMember" />.
        /// </summary>
        /// <param name="applicationId">Application that the user is a member of.</param>
        /// <param name="emailAddress">Email address to the member</param>
        public ApplicationTeamMember(int applicationId, string emailAddress)
        {
            if (applicationId <= 0) throw new ArgumentNullException("applicationId");
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            ApplicationId = applicationId;
            EmailAddress = emailAddress;
            AddedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected ApplicationTeamMember()
        {
        }

        /// <summary>
        ///     0 for invited users that do not have an existing account (and have not accepted the invitation yet).
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        ///     When the member was added, or when the invitation was sent.
        /// </summary>
        public DateTime AddedAtUtc { get; private set; }

        /// <summary>
        ///     User who added or invited this user.
        /// </summary>
        public string AddedByName { get; set; }

        /// <summary>
        ///     Application that the user is a member of.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        ///     Email address (if this is an invite for a non-existing user).
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        ///     Currently assigned roles
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "I like my arrays.")]
        public string[] Roles { get; set; }

        /// <summary>
        ///     Only used when fetching a member
        /// </summary>
        public string UserName { get; set; }
    }
}