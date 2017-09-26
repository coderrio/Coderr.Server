using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Applications.Commands;

namespace OneTrueError.App.Core.Applications.CommandHandlers
{
    /// <summary>
    ///     Used to update application name and applicationType.
    /// </summary>
    [Component]
    public class UpdateApplicationHandler : ICommandHandler<UpdateApplication>
    {
        private readonly IApplicationRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdateApplicationHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        public UpdateApplicationHandler(IApplicationRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(UpdateApplication command)
        {
            var app = await _repository.GetByIdAsync(command.ApplicationId);
            app.Name = command.Name;
            if (command.TypeOfApplication != null)
                app.ApplicationType = command.TypeOfApplication.Value;
            await _repository.UpdateAsync(app);
        }
    }
}