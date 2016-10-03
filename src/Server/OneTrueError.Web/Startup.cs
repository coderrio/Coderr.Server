using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(OneTrueError.Web.Startup))]

namespace OneTrueError.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(VirtualPathUtility.ToAbsolute("~/Account/Login")),
                CookieSecure = CookieSecureOption.SameAsRequest,
                Provider = new CookieAuthenticationProvider()
                {
                    OnApplyRedirect = ctx =>
                    {
                        if (!IsApiRequest(ctx.Request) && !ctx.Request.Headers.ContainsKey("X-Requested-With"))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            });

            ConfigureAuth(app);
        }
        private static bool IsApiRequest(IOwinRequest request)
        {
            string apiPath = VirtualPathUtility.ToAbsolute("~/api/");
            return request.Uri.LocalPath.StartsWith(apiPath);
        }
    }
}
