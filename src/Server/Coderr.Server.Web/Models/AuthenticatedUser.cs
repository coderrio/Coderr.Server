using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using codeRR.Server.Api.Core.Applications;

namespace codeRR.Server.Web.Models
{
    public class AuthenticatedUser
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public ApplicationListItem[] Applications { get; set; }
    }
}