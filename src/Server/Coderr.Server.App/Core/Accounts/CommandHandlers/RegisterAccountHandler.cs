using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Commands;
using codeRR.Server.Api.Core.Accounts.Events;
using codeRR.Server.Api.Core.Messaging;
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
    ///     Register a new account.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Will wait for activation before allowing the user to login.
    ///     </para>
    /// </remarks>
    [Component]
    public class RegisterAccountHandler : IMessageHandler<RegisterAccount>
    {
        private readonly IAccountRepository _repository;
        private ILog _logger = LogManager.GetLogger(typeof(RegisterAccountHandler));
        private ConfigurationStore _configStore;

        /// <summary>
        ///     Creates a new instance of <see cref="RegisterAccountHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        public RegisterAccountHandler(IAccountRepository repository, ConfigurationStore configStore)
        {
            _repository = repository;
            _configStore = configStore;
        }

        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, RegisterAccount command)
        {
            if (command == null) throw new ArgumentNullException("command");

            if (!string.IsNullOrEmpty(command.UserName) && await _repository.IsUserNameTakenAsync(command.UserName))
            {
                await SendAccountInfo(context, command.UserName);
                _logger.Warn("UserName is taken: " + command.UserName);
                return;
            }

            var account = command.AccountId > 0
                ? new Account(command.AccountId, command.UserName, command.Password)
                : new Account(command.UserName, command.Password);
            account.SetVerifiedEmail(command.Email);

            if (command.ActivateDirectly)
                account.Activate();

            var accountCount = await _repository.CountAsync();
            if (accountCount == 0)
                account.IsSysAdmin = true;

            await _repository.CreateAsync(account);

            // accounts can be activated directly.
            // should not send activation email then.
            if (account.AccountState == AccountState.VerificationRequired)
                await SendVerificationEmail(context, account);

            var evt = new AccountRegistered(account.Id, account.UserName) { IsSysAdmin = account.IsSysAdmin };
            await context.SendAsync(evt);

            if (command.ActivateDirectly)
            {
                var evt1 = new AccountActivated(account.Id, account.UserName) { EmailAddress = account.Email };
                await context.SendAsync(evt1);
            }
        }

        private async Task SendAccountInfo(IMessageContext context, string userName)
        {
            var account = await _repository.GetByUserNameAsync(userName);

            var config = _configStore.Load<BaseConfiguration>();
            //TODO: HTML email
            var msg = new EmailMessage
            {
                TextBody = @"Hello!

Someone (you?) tried to create an account with the same information as your account. 

If it was you, you can request a new password from the login page. Otherwise, 
contact us so that we can investigate: help@coderrapp.com

Cheerio,
  codeRR Team",
                Subject = "codeRR registration"
            };
            msg.Recipients = new[] { new EmailAddress(account.Email) };

            await context.SendAsync(new SendEmail(msg));
        }

        private Task SendVerificationEmail(IMessageContext context, Account account)
        {
            var config = _configStore.Load<BaseConfiguration>();
            //TODO: HTML email
            var msg = new EmailMessage
            {
                TextBody = string.Format(@"Welcome,


Your activation code is: {0}

You can activate your account by clicking on: {1}/account/activate/{0}

Good luck,
  codeRR Team", account.ActivationKey, config.BaseUrl),
                Subject = "codeRR activation"
            };
            msg.Recipients = new[] { new EmailAddress(account.Email) };

            return context.SendAsync(new SendEmail(msg));
        }
    }
}