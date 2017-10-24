using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Users.Commands;
using codeRR.Server.App.Core.Accounts;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Users.WebApi
{
    /// <summary>
    ///     Handler for <see cref="UpdatePersonalSettings" />.
    /// </summary>
    [Component]
    public class UpdatePersonalSettingsHandler : IMessageHandler<UpdatePersonalSettings>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdatePersonalSettingsHandler" />.
        /// </summary>
        /// <param name="userRepository">repos</param>
        /// <param name="accountRepository">Used to change email</param>
        /// <exception cref="ArgumentNullException">userRepository</exception>
        public UpdatePersonalSettingsHandler(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, UpdatePersonalSettings command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var user = await _userRepository.GetUserAsync(command.UserId);
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.MobileNumber = command.MobileNumber;

            if (command.EmailAddress != null)
                user.EmailAddress = command.EmailAddress;

            await _userRepository.UpdateAsync(user);

            if (command.EmailAddress == null)
                return;

            var account = await _accountRepository.GetByIdAsync(user.AccountId);
            account.SetVerifiedEmail(command.EmailAddress);
            await _accountRepository.UpdateAsync(account);
        }
    }
}