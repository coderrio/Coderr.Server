using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Commands;
using codeRR.Server.Api.Core.Accounts.Events;
using codeRR.Server.Api.Core.Messaging;
using codeRR.Server.Api.Core.Messaging.Commands;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Configuration;
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
    public class RegisterAccountHandler : ICommandHandler<RegisterAccount>
    {
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;
        private readonly IAccountRepository _repository;
        private ILog _logger = LogManager.GetLogger(typeof(RegisterAccountHandler));

        /// <summary>
        ///     Creates a new instance of <see cref="RegisterAccountHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <param name="commandBus">to send verification email</param>
        /// <param name="eventBus">to send <see cref="AccountRegistered" />.</param>
        public RegisterAccountHandler(IAccountRepository repository, ICommandBus commandBus, IEventBus eventBus)
        {
            _repository = repository;
            _commandBus = commandBus;
            _eventBus = eventBus;
        }

        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task ExecuteAsync(RegisterAccount command)
        {
            if (command == null) throw new ArgumentNullException("command");

            if (!string.IsNullOrEmpty(command.UserName) && await _repository.IsUserNameTakenAsync(command.UserName))
            {
                await SendAccountInfo(command.UserName);
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
                await SendVerificationEmail(account);

            var evt = new AccountRegistered(account.Id, account.UserName) { IsSysAdmin = account.IsSysAdmin };
            await _eventBus.PublishAsync(evt);

            if (command.ActivateDirectly)
            {
                var evt1 = new AccountActivated(account.Id, account.UserName) { EmailAddress = account.Email };
                await _eventBus.PublishAsync(evt1);
            }
        }

#pragma warning disable 1998
        private async Task SendAccountInfo(string userName)
#pragma warning restore 1998
        {
            var account = await _repository.GetByUserNameAsync(userName);

            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();
            //TODO: HTML email
            var msg = new EmailMessage
            {
                TextBody = @"Hello!

Someone (you?) tried to create an account with the same information as your account. 

If it was you, you can request a new password from the login page. Otherwise, 
contact us so that we can investigate: support@coderrapp.com

Cheerio,
  codeRR Team",
                Subject = "codeRR registration"
            };
            msg.Recipients = new[] { new EmailAddress(account.Email) };

            await _commandBus.ExecuteAsync(new SendEmail(msg));
        }

        private Task SendVerificationEmail(Account account)
        {
            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();
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

            return _commandBus.ExecuteAsync(new SendEmail(msg));
        }
    }
}