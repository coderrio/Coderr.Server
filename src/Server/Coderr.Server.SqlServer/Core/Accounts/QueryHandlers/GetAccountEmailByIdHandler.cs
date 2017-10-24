using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Queries;
using codeRR.Server.App.Core.Accounts;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.SqlServer.Core.Accounts.QueryHandlers
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

        public async Task<string> HandleAsync(IMessageContext context, GetAccountEmailById query)
        {
            var usr = await _accountRepository.GetByIdAsync((int) query.AccountId);
            return usr.Email;
        }
    }
}