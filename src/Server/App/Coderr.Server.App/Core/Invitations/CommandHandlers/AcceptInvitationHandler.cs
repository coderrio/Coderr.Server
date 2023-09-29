using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Accounts.Requests;
using Coderr.Server.App.Core.Accounts;
using DotNetCqs;


namespace Coderr.Server.App.Core.Invitations.CommandHandlers
{
    public class AcceptInvitationHandler : IMessageHandler<AcceptInvitation>
    {
        private readonly IAccountService _accountService;

        public AcceptInvitationHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task HandleAsync(IMessageContext context, AcceptInvitation message)
        {
            await _accountService.AcceptInvitation(context.Principal, message);
        }
    }
}
