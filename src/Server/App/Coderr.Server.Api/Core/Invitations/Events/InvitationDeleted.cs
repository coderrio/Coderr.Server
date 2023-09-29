using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Invitations.Events
{
    [Event]
    public class InvitationDeleted
    {
        public int InvitationId { get; set; }
        public string InvitedEmailAddress { get; set; }
        public int[] ApplicationIds { get; set; }
    }
}
