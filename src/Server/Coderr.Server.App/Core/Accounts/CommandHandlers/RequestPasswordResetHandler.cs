using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Accounts.Commands;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.Infrastructure.Configuration;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using log4net;

namespace Coderr.Server.App.Core.Accounts.CommandHandlers
{
    /// <summary>
    ///     Handler for <see cref="RequestPasswordReset" />.
    /// </summary>
    internal class RequestPasswordResetHandler : IMessageHandler<RequestPasswordReset>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly BaseConfiguration _baseConfig;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RequestPasswordResetHandler));

        public RequestPasswordResetHandler(IAccountRepository accountRepository, IConfiguration<BaseConfiguration> baseConfig)
        {
            _accountRepository = accountRepository;
            _baseConfig = baseConfig.Value;
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

            var cmd = new SendTemplateEmail("Password reset", "ResetPassword")
            {
                To = account.Email,
                Model =
                    new
                    {
                        AccountName = account.UserName,
                        ResetLink = 
                            _baseConfig.BaseUrl + "/password/reset/" +
                            account.ActivationKey
                    },
                Subject = "Reset password"
            };

            await context.SendAsync(cmd);
        }
    }
}