using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Accounts.Requests;

namespace OneTrueError.App.Core.Accounts.Requests
{
    /// <summary>
    ///     Handler for <see cref="ResetPassword" />.
    /// </summary>
    [Component]
    public class ResetPasswordHandler : IRequestHandler<ResetPassword, ResetPasswordReply>
    {
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="ResetPasswordHandler" />.
        /// </summary>
        /// <param name="accountRepository">accountRepository</param>
        /// <exception cref="ArgumentNullException">accountRepository</exception>
        public ResetPasswordHandler(IAccountRepository accountRepository)
        {
            if (accountRepository == null) throw new ArgumentNullException("accountRepository");
            _accountRepository = accountRepository;
        }

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="request">Request to execute</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        public async Task<ResetPasswordReply> ExecuteAsync(ResetPassword request)
        {
            var account = await _accountRepository.FindByActivationKeyAsync(request.ActivationKey);
            account.ChangePassword(request.NewPassword);
            await _accountRepository.UpdateAsync(account);
            return new ResetPasswordReply();
        }
    }
}