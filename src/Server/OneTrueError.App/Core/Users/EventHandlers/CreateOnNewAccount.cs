using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Accounts.Events;

namespace OneTrueError.App.Core.Users.EventHandlers
{
    /// <summary>
    ///     Responsible of creating an user entity when a new account is created.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    internal class CreateOnNewAccount : IApplicationEventSubscriber<AccountActivated>
    {
        private readonly IUserRepository _userRepository;

        public CreateOnNewAccount(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(AccountActivated e)
        {
            await _userRepository.CreateAsync(new User(e.AccountId, e.UserName)
            {
                EmailAddress = e.EmailAddress
            });
        }
    }
}