using System.Security.Claims;
using Coderr.Server.WebSite.Infrastructure.Boot;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coderr.Server.WebSite.Infrastructure
{
    public class PrincipalActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //DependencyResolver.Current.
            if (filterContext.HttpContext.User is ClaimsPrincipal principal)
            {
                var setter = (PrincipalWrapper)filterContext.HttpContext.RequestServices.GetService(typeof(PrincipalWrapper));
                setter.Principal = principal;
            }
                
            base.OnActionExecuting(filterContext);
        }
    }
}