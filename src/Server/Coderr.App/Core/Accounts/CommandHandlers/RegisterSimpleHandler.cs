using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using OneTrueError.Api.Core.Accounts;
using OneTrueError.Api.Core.Accounts.Commands;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.Api.Core.Messaging;
using OneTrueError.Api.Core.Messaging.Commands;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Core.Accounts.CommandHandlers
{
    /// <summary>
    ///     Handler for <see cref="RegisterSimple" />.
    /// </summary>
    [Component]
    internal class RegisterSimpleHandler : ICommandHandler<RegisterSimple>
    {
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RegisterSimpleHandler));
        private readonly IAccountRepository _repository;

        public RegisterSimpleHandler(IAccountRepository repository, ICommandBus commandBus, IEventBus eventBus)
        {
            _repository = repository;
            _commandBus = commandBus;
            _eventBus = eventBus;
        }

        public async Task ExecuteAsync(RegisterSimple command)
        {
            var pos = command.EmailAddress.IndexOf('@');
            if (pos == -1)
            {
                _logger.Warn("Invalid email address: " + command.EmailAddress);
                throw new InvalidOperationException("Invalid email address");
            }

            var user = _repository.FindByEmailAsync(command.EmailAddress);
            if (user != null)
            {
                _logger.Warn("Email already taken, sending reset password: " + command.EmailAddress);
                await _commandBus.ExecuteAsync(new RequestPasswordReset(command.EmailAddress));
            }

            var userName = await TryCreateUsernameAsync(command, pos);
            if (userName == null)
            {
                _logger.Error("Failed to generate username for " + command.EmailAddress);
                return;
            }


            //var id = _idGeneratorClient.GetNextId(Account.SEQUENCE);
            var password = Guid.NewGuid().ToString("N").Substring(0, 10);
            var account = new Account(userName, password);
            account.SetVerifiedEmail(command.EmailAddress);
            await _repository.CreateAsync(account);

            await SendAccountEmail(account, password);

            var evt = new AccountRegistered(account.Id, account.UserName);
            await _eventBus.PublishAsync(evt);
        }

        private Task SendAccountEmail(Account account, string password)
        {
            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();
            //TODO: HTML email
            var msg = new EmailMessage
            {
                TextBody = string.Format(@"Welcome,


We have created your account.

UserName: {1}
Password: {2}

You can login using {0}/account/activate/{3}.

We recommend that you change your password before doing something useful.

Thanks,
  The OneTrueError Team", config.BaseUrl, account.UserName, password, account.ActivationKey),
                Subject = "OneTrueError activation"
            };
            msg.Recipients = new[] {new EmailAddress(account.Email)};

            return _commandBus.ExecuteAsync(new SendEmail(msg));
        }

        private async Task<string> TryCreateUsernameAsync(RegisterSimple command, int pos)
        {
            var suggestedUserName = command.EmailAddress.Substring(0, pos);
            if (!await _repository.IsUserNameTakenAsync(suggestedUserName))
                return suggestedUserName;

            var counter = 100;
            var newUserName = suggestedUserName + counter;
            while (counter < 110)
            {
                if (!await _repository.IsUserNameTakenAsync(newUserName))
                {
                    suggestedUserName = newUserName;
                    return suggestedUserName;
                }

                counter++;
                newUserName = suggestedUserName + counter;
            }

            _logger.Error("Failed to generate userName: " + suggestedUserName);
            return null;
        }
    }
}