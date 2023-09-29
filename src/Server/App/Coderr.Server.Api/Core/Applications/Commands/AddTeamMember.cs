using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Applications.Commands
{
    [Command]
    public class AddTeamMember
    {
        public int UserToAdd { get; set; }
        public int ApplicationId { get; set; }

        public string[] Roles { get; set; }
    }
}
