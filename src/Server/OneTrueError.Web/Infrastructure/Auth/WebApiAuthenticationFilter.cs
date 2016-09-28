using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Reflection;
using System.Linq;
using OneTrueError.App;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Infrastructure
{
    public class WebApiAuthenticationFilter : IAuthenticationFilter
    {
        public bool AllowMultiple { get { return true; } }

#pragma warning disable 1998
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            if (!SessionUser.IsAuthenticated)
            {
                var attr = context.ActionContext.ControllerContext.Controller.GetType().GetCustomAttribute<AllowAnonymousAttribute>()
                    ?? context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().FirstOrDefault();
                if (attr == null)
                    context.ErrorResult = new AuthenticationFailureResult("Authenticate", context.Request);

                return;
            }

            var roles = SessionUser.Current.GetRoles();
            Thread.CurrentPrincipal = new OneTruePrincipal(SessionUser.Current.AccountId, SessionUser.Current.UserName,
                roles);
            context.Principal = Thread.CurrentPrincipal;
        }

#pragma warning disable 1998
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
        }
    }
}