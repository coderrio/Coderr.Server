using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Core.Accounts.Queries;
using codeRR.App.Core.Accounts;

namespace codeRR.SqlServer.Core.Accounts.QueryHandlers
{
    [Component]
    public class GetAccountEmailByIdHandler : IQueryHandler<GetAccountEmailById, string>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountEmailByIdHandler(IAccountRepository accountRepository)
        {
            if (accountRepository == null) throw new ArgumentNullException(nameof(accountRepository));
            _accountRepository = accountRepository;
        }

        public async Task<string> ExecuteAsync(GetAccountEmailById query)
        {
            var usr = await _accountRepository.GetByIdAsync(query.AccountId);
            return usr.Email;
        }
    }
}