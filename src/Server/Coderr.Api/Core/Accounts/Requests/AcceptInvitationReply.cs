using System;

namespace codeRR.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Reply for <see cref="AcceptInvitation" />.
    /// </summary>
    public class AcceptInvitationReply
    {
        /// <summary>
        ///     Creates a new instance of <see cref="AcceptInvitationReply" />.
        /// </summary>
        /// <param name="accountId">Primary key for the generated account</param>
        /// <param name="userName">Username</param>
        public AcceptInvitationReply(int accountId, string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");

            AccountId = accountId;
            UserName = userName;
        }

        /// <summary>
        ///     Primary key for the generated account
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        ///     Username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", UserName, AccountId);
        }
    }
}