using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Applications.Queries;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Applications.QueryHandlers
{
    [Component]
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
                    UserName = x.UserName
                }).ToArray()
            };
            return result;
        }
    }
}