using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Reflection;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DotNetCqs;
using codeRR.Api.Core.Accounts.Requests;
using codeRR.Web.Models;

namespace codeRR.Web.Infrastructure
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
                if (attr != null)
                {
                    return;
                }


                //if (TryBasicAuthentication(context))
                //{
                //    return;
                //}

                    context.ErrorResult = new AuthenticationFailureResult("Authenticate", context.Request);

                return;
            }

            Thread.CurrentPrincipal = new codeRRPrincipal
            {
                AccountId = SessionUser.Current.AccountId,
                ApplicationId = SessionUser.Current.ApplicationId,
                Identity = new GenericIdentity(SessionUser.Current.UserName),
            };
            context.Principal = Thread.CurrentPrincipal;
        }

        //private async Task<bool> TryBasicAuthentication(HttpAuthenticationContext context)
        //{
        //    HttpRequestMessage request = context.Request;
        //    AuthenticationHeaderValue authorization = request.Headers.Authorization;

        //    if (authorization == null || authorization.Scheme != "Basic")
        //    {
        //        return false;
        //    }

        //    if (string.IsNullOrEmpty(authorization.Parameter))
        //    {
        //        context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
        //        return false;
        //    }

        //    var encoding = Encoding.GetEncoding("iso-8859-1");
        //    var credentials = encoding.GetString(Convert.FromBase64String(authorization.Parameter));

        //    int separator = credentials.IndexOf(':');
        //    string userName = credentials.Substring(0, separator);
        //    string password = credentials.Substring(separator + 1);

        //    var principal = await AuthenticateUserAsync(userName, password);
        //    if (principal == null)
        //    {
        //        context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
        //        return false;
        //    }

        //    context.Principal = principal;
        //    return true;
        //}

        //private Task<IPrincipal> AuthenticateUserAsync(string userName, string password)
        //{
        //    using (var scope = GlobalConfiguration.Configuration.DependencyResolver.BeginScope())
        //    {
        //        var bus = (IRequestReplyBus) scope.GetService(typeof(IRequestReplyBus));
        //        await Results = bus.ExecuteAsync(new Login(userName, password));
        //    }
        //}

#pragma warning disable 1998
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
        }
    }
}