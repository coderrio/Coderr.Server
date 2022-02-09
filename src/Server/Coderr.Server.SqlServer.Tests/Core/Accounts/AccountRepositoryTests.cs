using System.Threading.Tasks;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.SqlServer.Core.Accounts;
using Coderr.Server.SqlServer.Tests;
using FluentAssertions;
using Griffin.Data;
using Xunit;
using Xunit.Abstractions;

namespace Coderr.Server.SqlServer.Tests.Core.Accounts
{
    public class AccountRepositoryTests : IntegrationTest
    {

        public AccountRepositoryTests(ITestOutputHelper helper) : base(helper)
        {
        }


        [Fact]
        public async Task Should_include_id_when_specified_in_the_account_entity()
        {
            return; //TODO: Requires a modified version of the DB (since Lobby generates accounts in Coderr Live)
            var account = new Account(43, "arne", "pass");
            using (var uow = CreateUnitOfWork())
            {
                var repos = new AccountRepository(uow);
                await repos.CreateAsync(account);
                uow.SaveChanges();
            }


            using (var uow2 = CreateUnitOfWork())
            {
                var repos2 = new AccountRepository(uow2);
                await repos2.GetByIdAsync(43);
            }
        }
    }
}
