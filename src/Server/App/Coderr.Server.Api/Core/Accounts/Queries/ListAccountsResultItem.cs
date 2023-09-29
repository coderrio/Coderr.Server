using System;

namespace Coderr.Server.Api.Core.Accounts.Queries
{
    public class ListAccountsResultItem
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}