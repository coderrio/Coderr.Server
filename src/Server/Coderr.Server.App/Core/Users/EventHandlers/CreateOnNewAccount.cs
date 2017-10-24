using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Events;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Users.EventHandlers
{
    /// <summary>
    ///     Responsible of creating an user entity when a new account is created.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    internal class CreateOnNewAccount : IMessageHandler<AccountActivated>
    {
        private readonly IUserRepository _userRepository;

        public CreateOnNewAccount(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(IMessageContext context, AccountActivated e)
        {
            await _userRepository.CreateAsync(new User(e.AccountId, e.UserName)
            {
                EmailAddress = e.EmailAddress
            });
        }
    }
}