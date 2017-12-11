using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Commands;
using codeRR.Server.Api.Core.Messaging.Commands;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Core.Accounts.CommandHandlers
{
    /// <summary>
    ///     Handler for <see cref="RequestPasswordReset" />.
    /// </summary>
    [Component]
    internal class RequestPasswordResetHandler : IMessageHandler<RequestPasswordReset>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RequestPasswordResetHandler));
        private ConfigurationStore _configStore;

        public RequestPasswordResetHandler(IAccountRepository accountRepository, ConfigurationStore configStore)
        {
            _accountRepository = accountRepository;
            _configStore = configStore;
        }

        public async Task HandleAsync(IMessageContext context, RequestPasswordReset command)
        {
            var account = await _accountRepository.FindByEmailAsync(command.EmailAddress);
            if (account == null)
            {
                _logger.Warn("Failed to find a user with email " + command.EmailAddress);
                return;
            }

            account.RequestPasswordReset();
            await _accountRepository.UpdateAsync(account);

            var config = _configStore.Load<BaseConfiguration>();
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

            await context.SendAsync(cmd);
        }
    }
}