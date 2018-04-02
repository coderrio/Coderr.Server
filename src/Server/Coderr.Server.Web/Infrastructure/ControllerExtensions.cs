using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.Web.Infrastructure
{
    public static class ControllerExtensions
    {
        public static ClaimsPrincipal ClaimsUser(this Controller controller)
        {
            return (ClaimsPrincipal) controller.User;
        }
    }
}
