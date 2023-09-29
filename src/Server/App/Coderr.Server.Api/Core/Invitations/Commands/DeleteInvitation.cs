using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Invitations.Commands
{
    [Message]
    public class DeleteInvitation 
    {
        public int ApplicationId { get; set; }
        public string InvitedEmailAddress { get; set; }

    }
}
