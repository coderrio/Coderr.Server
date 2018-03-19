using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.Web2.Infrastrucutre
{
    public static class ControllerExtensions
    {
        public static ClaimsPrincipal ClaimsUser(this Controller controller)
        {
            return (ClaimsPrincipal) controller.User;
        }
    }
}
