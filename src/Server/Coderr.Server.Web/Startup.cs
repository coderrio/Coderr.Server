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
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Configuration.Database;
using codeRR.Server.Infrastructure.Security;
using codeRR.Server.SqlServer.Core.Users;
using codeRR.Server.Web;
using codeRR.Server.Web.Infrastructure;
using codeRR.Server.Web.Infrastructure.Auth;
using codeRR.Server.Web.Infrastructure.Logging;
using codeRR.Server.Web.Services;
using codeRR.Server.Web.Views;
using Griffin.Data.Mapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using codeRR.Client;
using codeRR.Server.SqlServer.Tools;
using Coderr.Server.PluginApi.Config;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace codeRR.Server.Web
{
    public class Startup
    {
        private readonly ServiceRunner _serviceRunner;

        public Startup()
        {
            _serviceRunner = new ServiceRunner();
            ConfigurationStore = new DatabaseStore(() => DbConnectionFactory.Open(true));
        }

        public static ConfigurationStore ConfigurationStore { get; private set; }

        private bool IsConfigured => ConfigurationManager.AppSettings["Configured"] == "true";

        public void Configuration(IAppBuilder app)
        {
            if (!IsConfigured)
            {
                RouteConfig.RegisterInstallationRoutes(RouteTable.Routes);
            }
            else
            {
                ConfigureErrorTracking(ConfigurationStore);
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

        private static void ConfigureApiKeyAuthentication(HttpFilterCollection configFilters)
        {
            var configType = ConfigurationManager.AppSettings["ApiKeyAuthenticator"];
            if (!string.IsNullOrEmpty(configType))
            {
                var instance = TypeHelper.CreateAssemblyObject(configType);
                configFilters.Add((IFilter)instance);
                return;
            }

            configFilters.Add(new ApiKeyAuthenticator());
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            var type = ConfigurationManager.AppSettings["PrincipalFactoryType"];
            if (type != null)
            {
                PrincipalFactory.Configure(type);
            }

            var provider = new CookieAuthenticationProvider
            {
                OnApplyRedirect = ctx =>
                {
                    if (!IsApiRequest(ctx.Request) && !ctx.Request.Headers.ContainsKey("X-Requested-With"))
                        ctx.Response.Redirect(ctx.RedirectUri);
                }
            };

            var loginUrl = "/Account/Login"; //VirtualPathUtility.ToAbsolute("~/Account/Login");
            BaseViewPage.LoginUrl = loginUrl;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromDays(14),
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

        private static void ConfigureErrorTracking(ConfigurationStore configStore)
        {
            var errorTrackingConfig = configStore.Load<codeRRConfigSection>();
            if (errorTrackingConfig.ActivateTracking)
            {
                var uri = new Uri("https://report.coderrapp.com");
                if (!string.IsNullOrEmpty(errorTrackingConfig.ContactEmail) ||
                    !string.IsNullOrEmpty(errorTrackingConfig.InstallationId))
                    Err.Configuration.ContextProviders.Add(new CustomerInfoProvider(
                        errorTrackingConfig.ContactEmail,
                        errorTrackingConfig.InstallationId));
                Err.Configuration.Credentials(uri,
                    "2b3002d3ab3e4a57ad45cff2210221ab",
                    "f381a5c9797f49bd8a3238b892d02806");
                GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new WebApiLogger());
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

            config.MessageHandlers.Add(new CompressedRequestHandler());
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
                middlewareRegistrar.GetType().GetMethod("Register").Invoke(middlewareRegistrar, new object[] { app });
        }
    }
}