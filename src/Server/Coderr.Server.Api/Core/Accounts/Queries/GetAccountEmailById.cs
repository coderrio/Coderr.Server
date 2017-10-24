using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Queries
{
    /// <summary>
    ///     Get email for a specific account
    /// </summary>
    [Message]
    public class GetAccountEmailById : Query<string>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetAccountById" />.
        /// </summary>
        /// <param name="accountId">account</param>
        public GetAccountEmailById(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException("accountId");
            AccountId = accountId;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetAccountEmailById()
        {
        }

        /// <summary>
        ///     Account
        /// </summary>
        public int AccountId { get; private set; }
    }
}