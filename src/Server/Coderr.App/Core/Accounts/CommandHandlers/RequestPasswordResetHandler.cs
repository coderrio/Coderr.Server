using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using codeRR.Api.Core.Accounts.Commands;
using codeRR.Api.Core.Messaging.Commands;
using codeRR.App.Configuration;
using codeRR.Infrastructure.Configuration;

namespace codeRR.App.Core.Accounts.CommandHandlers
{
    /// <summary>
    ///     Handler for <see cref="RequestPasswordReset" />.
    /// </summary>
    [Component]
    internal class RequestPasswordResetHandler : ICommandHandler<RequestPasswordReset>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICommandBus _commandBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RequestPasswordResetHandler));

        public RequestPasswordResetHandler(IAccountRepository accountRepository, ICommandBus commandBus)
        {
            _accountRepository = accountRepository;
            _commandBus = commandBus;
        }

        public async Task ExecuteAsync(RequestPasswordReset command)
        {
            var account = await _accountRepository.FindByEmailAsync(command.EmailAddress);
            if (account == null)
            {
                _logger.Warn("Failed to find a user with email " + command.EmailAddress);
                return;
            }

            account.RequestPasswordReset();
            await _accountRepository.UpdateAsync(account);

            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();
            var cmd = new SendTemplateEmail("Password reset", "ResetPassword")
            {
                To = account.Email,
                Model =
                    new
                    {
                        AccountName = account.UserName,
                        ResetLink = //TODO: Remove app settings dependency
                            config.BaseUrl + "/password/reset/" +
                            account.ActivationKey
                    },
                Subject = "Reset password"
            };

            await _commandBus.ExecuteAsync(cmd);
        }
    }
}