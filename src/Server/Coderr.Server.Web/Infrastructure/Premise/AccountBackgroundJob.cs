using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.Premise.App;
using Griffin.ApplicationServices;

namespace Coderr.Server.Web.Infrastructure.Premise
{
    public class AccountBackgroundJob : IBackgroundJobAsync
    {
        private IAccountRepository _accountRepository;

        public AccountBackgroundJob(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task ExecuteAsync()
        {
            var count = await _accountRepository.CountAsync();
            LicenseWrapper.Instance.UpdateNumberOfUserAccounts(count);
        }
    }
}
