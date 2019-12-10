using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Users.Commands
{
    [Message]
    public class UpdateBrowserSubscription
    {
        public int UserId { get; set; }
        public string Endpoint { get; set; }


        public DateTime ExpiresAtUtc { get; set; }
    }

}
