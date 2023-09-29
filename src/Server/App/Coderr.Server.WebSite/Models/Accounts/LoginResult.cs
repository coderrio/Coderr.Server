using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coderr.Server.WebSite.Models.Accounts
{
    public class LoginResult
    {
        public string JwtToken { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
    }
}
