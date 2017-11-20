using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Applications;
using codeRR.Server.Api.Core.Applications.Commands;
using codeRR.Server.Api.Core.Applications.Events;
using codeRR.Server.App.Core.Users;
using codeRR.Server.Infrastructure.Security;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Applications.CommandHandlers
{
    [Component]
    internal class CreateApplicationHandler : IMessageHandler<CreateApplication>
    {
        private readonly IApplicationRepository _repository;
        private readonly IUserRepository _userRepository;

        public CreateApplicationHandler(IApplicationRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(IMessageContext context, CreateApplication command)
        {
            var app = new Application(command.UserId, command.Name)
            {
                AppKey = command.ApplicationKey,
                ApplicationType =
                    (TypeOfApplication) Enum.Parse(typeof(TypeOfApplication), command.TypeOfApplication.ToString())
            };
            var creator = await _userRepository.GetUserAsync(command.UserId);

            await _repository.CreateAsync(app);
            await _repository.CreateAsync(new ApplicationTeamMember(app.Id, creator.AccountId, creator.UserName)
            {
                UserName = creator.UserName,
                Roles = new[] {ApplicationRole.Admin, ApplicationRole.Member},
            });

            var evt = new ApplicationCreated(app.Id, app.Name, command.UserId, command.ApplicationKey, app.SharedSecret);
            await context.SendAsync(evt);
        }
    }
}