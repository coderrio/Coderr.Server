using System;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.Data.Mapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using OneTrueError.App.Configuration;
using OneTrueError.Client;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.SqlServer.Core.Users;
using OneTrueError.Web;
using OneTrueError.Web.Infrastructure;
using OneTrueError.Web.Infrastructure.Auth;
using OneTrueError.Web.Infrastructure.Logging;
using OneTrueError.Web.Services;
using OneTrueError.Web.Views;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace OneTrueError.Web
{
    public class Startup
    {
        private readonly ServiceRunner _serviceRunner = new ServiceRunner();

        public void Configuration(IAppBuilder app)
        {
            if (!IsConfigured)
                RouteConfig.RegisterInstallationRoutes(RouteTable.Routes);
            else
            {
                ConfigureErrorTracking();
                ConfigureDataMapping();
                _serviceRunner.Start(app);
            }

            ConfigureAuth(app);
            LoadCustomAuthenticationMiddleware(app);
            app.Map("", ConfigureWebApi);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            if (IsConfigured)
                RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private bool IsConfigured
        {
            get { return ConfigurationManager.AppSettings["Configured"] == "true"; }

        }
        private static void ConfigureApiKeyAuthentication(HttpFilterCollection configFilters)
        {
            var configType = ConfigurationManager.AppSettings["ApiKeyAuthenticator"];
            if (!string.IsNullOrEmpty(configType))
            {
                var instance = TypeHelper.CreateAssemblyObject(configType);
                configFilters.Add((IFilter) instance);
                return;
            }

            configFilters.Add(new ApiKeyAuthenticator());
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            var provider = new CookieAuthenticationProvider
            {
                OnApplyRedirect = ctx =>
                {
                    if (!IsApiRequest(ctx.Request) && !ctx.Request.Headers.ContainsKey("X-Requested-With"))
                        ctx.Response.Redirect(ctx.RedirectUri);
                }
            };

            var loginUrl = VirtualPathUtility.ToAbsolute("~/Account/Login");
            BaseViewPage.LoginUrl = loginUrl;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(loginUrl),
                Provider = provider,
                SessionStore = new SessionStoreMediator()
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }

        private static void ConfigureDataMapping()
        {
            var provider = new AssemblyScanningMappingProvider();
            provider.Scan(typeof(UserMapper).Assembly);
            EntityMappingProvider.Provider = provider;
        }

        private static void ConfigureErrorTracking()
        {
            var errorTrackingConfig = ConfigurationStore.Instance.Load<OneTrueErrorConfigSection>();
            if ((errorTrackingConfig != null) && errorTrackingConfig.ActivateTracking)
            {
                var uri = new Uri("http://reporting.onetrueerror.com");
                if (!string.IsNullOrEmpty(errorTrackingConfig.ContactEmail) ||
                    !string.IsNullOrEmpty(errorTrackingConfig.InstallationId))
                    OneTrue.Configuration.ContextProviders.Add(new CustomerInfoProvider(
                        errorTrackingConfig.ContactEmail,
                        errorTrackingConfig.InstallationId));
                OneTrue.Configuration.Credentials(uri, "appKey", "sharedSecret");
            }
            else
            {
                GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new WebApiLogger());
            }
        }

        private static void ConfigureWebApi(IAppBuilder inner)
        {
            var config = new HttpConfiguration();
            ConfigureApiKeyAuthentication(config.Filters);
            inner.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                SessionStore = new SessionStoreMediator()
            });

            config.DependencyResolver = GlobalConfiguration.Configuration.DependencyResolver;
            config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute(
            //    "DefaultApi",
            //    "{controller}/{id}",
            //    new {id = RouteParameter.Optional}
            //);

            inner.UseWebApi(config);
        }

        private static bool IsApiRequest(IOwinRequest request)
        {
            var apiPath = VirtualPathUtility.ToAbsolute("~/api/");
            return request.Uri.LocalPath.StartsWith(apiPath);
        }

        private static void LoadCustomAuthenticationMiddleware(IAppBuilder app)
        {
            var middlewareTypeStr = ConfigurationManager.AppSettings["AuthenticationMiddleware"];
            if (middlewareTypeStr == null)
                return;

            var middlewareRegistrar = TypeHelper.CreateAssemblyObject(middlewareTypeStr);
            if (middlewareRegistrar != null)
                middlewareRegistrar.GetType().GetMethod("Register").Invoke(middlewareRegistrar, new object[] {app});
        }
    }
}