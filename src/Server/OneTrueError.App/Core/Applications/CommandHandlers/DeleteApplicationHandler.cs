using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Applications.Commands;
using OneTrueError.Api.Core.Applications.Events;
using OneTrueError.Infrastructure.Security;

namespace OneTrueError.App.Core.Applications.CommandHandlers
{
    /// <summary>
    ///     Handler for <see cref="DeleteApplication" />.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class DeleteApplicationHandler : ICommandHandler<DeleteApplication>
    {
        private readonly IEventBus _eventBus;
        private readonly IApplicationRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteApplicationHandler" />.
        /// </summary>
        /// <param name="repository">used to delete the application</param>
        /// <param name="eventBus">to publish ApplicationDeleted</param>
        public DeleteApplicationHandler(IApplicationRepository repository, IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(DeleteApplication command)
        {
            ClaimsPrincipal.Current.EnsureApplicationAdmin(command.Id);

            var app = await _repository.GetByIdAsync(command.Id);
            await _repository.DeleteAsync(command.Id);
            var evt = new ApplicationDeleted {ApplicationName = app.Name, ApplicationId = app.Id, AppKey = app.AppKey};
            await _eventBus.PublishAsync(evt);
        }
    }
}