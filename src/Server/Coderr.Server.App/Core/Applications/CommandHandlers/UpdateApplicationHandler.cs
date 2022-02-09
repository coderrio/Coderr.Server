using System.Threading.Tasks;
using Coderr.Server.Api;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;


namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    /// <summary>
    ///     Used to update application name and applicationType.
    /// </summary>
    public class UpdateApplicationHandler : IMessageHandler<UpdateApplication>
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
        public async Task HandleAsync(IMessageContext context, UpdateApplication command)
        {
            var app = await _repository.GetByIdAsync(command.ApplicationId);
            app.Name = command.Name;
            if (command.TypeOfApplication != null)
            {
                app.ApplicationType =  command.TypeOfApplication.Value.ConvertEnum<TypeOfApplication>();
            }

            app.RetentionDays = command.RetentionDays ?? 60;
                
            await _repository.UpdateAsync(app);
        }
    }
}