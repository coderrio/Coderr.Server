using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.App.Core.Applications;
using OneTrueError.App.Core.Users;

namespace OneTrueError.App.Core.Invitations.EventHandlers
{
    /// <summary>
    ///     Responsible of updating the application membership when the user accepted the OTE invite.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class InvitationAcceptedHandler : IApplicationEventSubscriber<InvitationAccepted>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (InvitationAcceptedHandler));
        private readonly IApplicationRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="InvitationAcceptedHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public InvitationAcceptedHandler(IApplicationRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(InvitationAccepted e)
        {
            if (e == null) throw new ArgumentNullException("e");

            foreach (var applicationId in e.ApplicationIds)
            {
                var members = await _repository.GetTeamMembersAsync(applicationId);

                //need to use email as the user might not have existed
                // when the invitation was created.
                var member = members.FirstOrDefault(x => x.EmailAddress == e.EmailAddress);
                if (member == null)
                {
                    _logger.Error("Failed to find user " + e.EmailAddress + " for appId " + applicationId);
                    member = new ApplicationTeamMember(applicationId, e.EmailAddress)
                    {
                        AccountId = e.AccountId,
                        AddedByName = e.InvitedByUserName,
                        UserName = e.UserName,
                        Roles = new[] {ApplicationRole.Member}
                    };
                    await _repository.UpdateAsync(member);
                }

                member.AccountId = e.AccountId;
                if (member.Roles == null || member.Roles.Length == 0)
                    member.Roles = new[] {ApplicationRole.Member};
                await _repository.UpdateAsync(member);
            }
        }
    }
}