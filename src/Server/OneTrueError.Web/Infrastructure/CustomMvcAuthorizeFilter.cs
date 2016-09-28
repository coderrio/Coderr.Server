using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OneTrueError.App;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomMvcAuthorizeFilter : AuthorizeAttribute
    {
        public string RedirectUrl = "~/Account/Login";

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //do not load principal for activation url
            //as for some reason that means that the incorrect database will be used
            //during data population.
            var activateUrl = VirtualPathUtility.ToAbsolute("~/account/activate");
            if (httpContext.Request.Url.AbsolutePath.StartsWith(activateUrl))
                return true;

            if (!SessionUser.IsAuthenticated)
            {
                HttpCookie authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    try
                    {
                        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                        if (ticket != null && (!ticket.IsPersistent || !ticket.Expired))
                        {
                            httpContext.Response.Redirect(VirtualPathUtility.ToAbsolute("~/account/cookielogin?ReturnTo=" + httpContext.Request.Url.AbsolutePath + "?" + httpContext.Request.QueryString));
                            return true;
                        }
                    }
                    catch (CryptographicException)
                    {
                        //cookie was created on another server.
                        FormsAuthentication.SignOut();
                        httpContext.Response.Redirect(VirtualPathUtility.ToAbsolute("~/account/cookielogin?ReturnTo=" + httpContext.Request.Url.AbsolutePath + "?" + httpContext.Request.QueryString));
                        return true;
                    }

                }
            }

            if (SessionUser.IsAuthenticated)
            {
                Thread.CurrentPrincipal = new OneTruePrincipal(SessionUser.Current.AccountId,
                    SessionUser.Current.UserName, SessionUser.Current.GetRoles());
                httpContext.User = Thread.CurrentPrincipal;
            }


            var result = base.AuthorizeCore(httpContext);
            if (result)
            {
                var isAdminPath = httpContext.Request.Url.AbsolutePath.StartsWith("/admin",
                    StringComparison.OrdinalIgnoreCase);
                if (SessionUser.Current.AccountId != 1 && isAdminPath)
                    return false;
            }
            return result;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (!filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.RequestContext.HttpContext.Response.Redirect(RedirectUrl);
            }
        }
    }
}