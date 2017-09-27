using System;

namespace codeRR.Server.Api.Core.Accounts.Queries
{
    /// <summary>
    ///     Account entity subset.
    /// </summary>
    public class AccountDTO
    {
        /// <summary>
        ///     When the account was created
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Associated email address.
        /// </summary>
        //TODO: add to mapping
        public string Email { get; set; }

        /// <summary>
        ///     Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Last time user logged in.
        /// </summary>
        public DateTime LastLoginAtUtc { get; set; }

        /// <summary>
        ///     Current account state
        /// </summary>
        public AccountState State { get; set; }

        /// <summary>
        ///     When the account was updated (changed first name etc)
        /// </summary>
        public DateTime UpdatedAtUtc { get; set; }

        /// <summary>
        ///     Username
        /// </summary>
        public string UserName { get; set; }
    }
}