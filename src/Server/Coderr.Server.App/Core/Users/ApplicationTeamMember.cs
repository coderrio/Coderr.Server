using System;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Core.Users
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
        /// <remarks>
        ///     <para>
        ///         this constructor is used when the user have no account (invite user)
        ///     </para>
        /// </remarks>
        public ApplicationTeamMember(int applicationId, string emailAddress)
        {
            if (applicationId <= 0) throw new ArgumentNullException("applicationId");
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            ApplicationId = applicationId;
            EmailAddress = emailAddress;
            AddedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ApplicationTeamMember" />.
        /// </summary>
        /// <param name="applicationId">Application that the user is a member of.</param>
        /// <param name="accountId">User exist in the system</param>
        /// <remarks>
        ///     <para>
        ///         this constructor is used when the user have an account.
        ///     </para>
        /// </remarks>
        public ApplicationTeamMember(int applicationId, int accountId, string addedByName)
        {
            if (applicationId <= 0) throw new ArgumentNullException("applicationId");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            ApplicationId = applicationId;
            AccountId = accountId;
            AddedAtUtc = DateTime.UtcNow;
            EmailAddress = "";
            AddedByName = addedByName;
            Roles = new[] {"Member"};
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
        public int AccountId { get; private set; }

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
        ///     Email address (should only be used if this is an invite for a non-existing user).
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        ///     PK for the mapping
        /// </summary>
        public int Id { get; set; }

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

        /// <summary>
        ///     Invitation have been accepted.
        /// </summary>
        /// <param name="accountId"></param>
        public void AcceptInvitation(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            AccountId = accountId;
            EmailAddress = null;
        }
    }
}