using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.Api.Core.Applications;
using OneTrueError.Api.Core.Applications.Commands;

namespace OneTrueError.App.Core.Applications.EventHandlers
{
    /// <summary>
    ///     Responsible of creating a sample application when a new account is registered.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    internal class CreateDefaultAppOnAccountActivated : IApplicationEventSubscriber<AccountActivated>
    {
        private readonly ICommandBus _commandBus;

        public CreateDefaultAppOnAccountActivated(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task HandleAsync(AccountActivated e)
        {
            await
                _commandBus.ExecuteAsync(new CreateApplication("SampleApp", TypeOfApplication.DesktopApplication)
                {
                    UserId = e.AccountId
                });
        }
    }
}