using System;
using codeRR.Server.App.Core.Accounts;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Accounts
{
    public class AccountMapper : CrudEntityMapper<Account>
    {
        public AccountMapper() : base("Accounts")
        {
            Property(x => x.Id)
                .PrimaryKey(true);

            Property(x => x.AccountState)
                .ToPropertyValue(o => (AccountState) Enum.Parse(typeof(AccountState), (string) o, true))
                .ToColumnValue(o => o.ToString());

            Property(x => x.UpdatedAtUtc)
                .ToColumnValue(DbConverters.ToSqlNull);

            Property(x => x.LastLoginAtUtc)
                .ToColumnValue(DbConverters.ToSqlNull);
        }
    }
}