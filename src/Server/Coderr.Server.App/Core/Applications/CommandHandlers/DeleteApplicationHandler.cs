using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Api.Core.Applications.Events;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;

namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    /// <summary>
    ///     Handler for <see cref="DeleteApplication" />.
    /// </summary>
    public class DeleteApplicationHandler : IMessageHandler<DeleteApplication>
    {
        private readonly IApplicationRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="DeleteApplicationHandler" />.
        /// </summary>
        /// <param name="repository">used to delete the application</param>
        public DeleteApplicationHandler(IApplicationRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(IMessageContext context, DeleteApplication command)
        {
            context.Principal.EnsureApplicationAdmin(command.Id);

            var app = await _repository.GetByIdAsync(command.Id);
            await _repository.DeleteAsync(command.Id);
            var evt = new ApplicationDeleted {ApplicationName = app.Name, ApplicationId = app.Id, AppKey = app.AppKey};
            await context.SendAsync(evt);
        }
    }
}