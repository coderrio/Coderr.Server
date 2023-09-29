using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;


namespace Coderr.Server.App.Core.Applications.QueryHandlers
{
    internal class GetApplicationTeamHandler : IQueryHandler<GetApplicationTeam, GetApplicationTeamResult>
    {
        private readonly IApplicationRepository _applicationRepository;

        public GetApplicationTeamHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<GetApplicationTeamResult> HandleAsync(IMessageContext context, GetApplicationTeam query)
        {
            var members = await _applicationRepository.GetTeamMembersAsync(query.ApplicationId);
            var result = new GetApplicationTeamResult
            {
                Invited = members.Where(x => x.AccountId == 0).Select(x => new GetApplicationTeamResultInvitation
                {
                    EmailAddress = x.EmailAddress,
                    InvitedAtUtc = x.AddedAtUtc,
                    InvitedByUserName = x.AddedByName
                }).ToArray(),
                Members = members.Where(x => x.AccountId != 0).Select(x => new GetApplicationTeamMember
                {
                    JoinedAtUtc = DateTime.UtcNow,
                    UserId = x.AccountId,
                    UserName = x.UserName,
                    IsAdmin = x.Roles.Any(y => y == "Admin")
                }).ToArray()
            };
            return result;
        }
    }
}