using System;

namespace codeRR.Server.Api.Core.Accounts.Queries
{
    /// <summary>
    ///     Result for <see cref="FindAccountByUserName" />.
    /// </summary>
    public class FindAccountByUserNameResult
    {
        /// <summary>
        ///     Creates a new instance of <see cref="FindAccountByUserNameResult" />.
        /// </summary>
        /// <param name="accountId">account id</param>
        /// <param name="displayName">Either username or FirstName LastName depending on what's available.</param>
        /// <exception cref="ArgumentNullException">displayName</exception>
        /// <exception cref="ArgumentOutOfRangeException">accountId</exception>
        public FindAccountByUserNameResult(int accountId, string displayName)
        {
            if (displayName == null) throw new ArgumentNullException("displayName");
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            AccountId = accountId;
            DisplayName = displayName;
        }

        /// <summary>
        ///     Account id
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        ///     Either username or FirstName LastName depending on what's available.
        /// </summary>
        public string DisplayName { get; private set; }
    }
}