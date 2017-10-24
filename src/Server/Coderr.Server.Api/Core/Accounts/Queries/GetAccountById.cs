using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Queries
{
    /// <summary>
    ///     Get account information.
    /// </summary>
    [Message]
    public class GetAccountById : Query<AccountDTO>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetAccountById" />.
        /// </summary>
        /// <param name="accountId">Account id.</param>
        public GetAccountById(int accountId)
        {
            if (accountId < 1)
                throw new ArgumentNullException("accountId");

            AccountId = accountId;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetAccountById()
        {
        }

        /// <summary>
        ///     Account id.
        /// </summary>
        public int AccountId { get; private set; }
    }
}