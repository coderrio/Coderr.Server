using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace codeRR.Server.Web.Controllers
{
    public static class ControllerExtensions
    {
        public static ClaimsPrincipal ClaimsUser(this Controller controller)
        {
            return (ClaimsPrincipal) controller.User;
        }
    }
}