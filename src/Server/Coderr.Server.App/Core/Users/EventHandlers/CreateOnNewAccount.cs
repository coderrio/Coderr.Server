using System.Threading.Tasks;
using Coderr.Server.Api.Core.Accounts.Events;
using Coderr.Server.Domain.Core.User;
using DotNetCqs;


namespace Coderr.Server.App.Core.Users.EventHandlers
{
    /// <summary>
    ///     Responsible of creating an user entity when a new account is created.
    /// </summary>
    internal class CreateOnNewAccount : IMessageHandler<AccountActivated>
    {
        private readonly IUserRepository _userRepository;

        public CreateOnNewAccount(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(IMessageContext context, AccountActivated e)
        {
            var user = await _userRepository.FindByEmailAsync(e.EmailAddress);
            if (user != null)
                return;
            
            await _userRepository.CreateAsync(new User(e.AccountId, e.UserName)
            {
                EmailAddress = e.EmailAddress
            });
        }
    }
}