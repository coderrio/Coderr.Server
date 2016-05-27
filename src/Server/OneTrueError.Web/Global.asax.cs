using System;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Griffin.Data.Mapper;
using log4net;
using log4net.Config;
using OneTrueError.App.Configuration;
using OneTrueError.Client;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.SqlServer;
using OneTrueError.SqlServer.Core.Users;
using OneTrueError.Web.Infrastructure;
using OneTrueError.Web.Infrastructure.Logging;
using OneTrueError.Web.Services;

namespace OneTrueError.Web
{
    public class WebApiApplication : HttpApplication
    {
        private static readonly ILog _logger;
        private readonly ServiceRunner _serviceRunner = new ServiceRunner();

        static WebApiApplication()
        {
            var path2 = AppDomain.CurrentDomain.BaseDirectory;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(path2, "log4net.config")));
            _logger = LogManager.GetLogger(typeof (WebApiApplication));
            _logger.Info("Loaded");
            Griffin.Logging.LogManager.Provider = new LogManagerAdapter();
        }


        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        protected void Application_Start()
        {
            _logger.Info("Started");

            SetupTools.DbTools = new SqlServerTools();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            if (InstallationHelper.IsInstallationRequired())
            {
                RouteConfig.RegisterAdminRoutes(RouteTable.Routes);
                return;
            }

            ConfigureErrorTracking();
            ConfigureStandardSetup();
        }

        private void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            var data = "";
            if (Request.InputStream != null && Request.InputStream.Length > 0)
            {
                var reader = new StreamReader(Request.InputStream);
                data = reader.ReadToEnd();
            }
            _logger.Error("Request + " + Request.Url + ", data" + data, exception);
        }

        private static void ConfigureErrorTracking()
        {
            var errorTrackingConfig = ConfigurationStore.Instance.Load<OneTrueErrorConfigSection>();
            if (errorTrackingConfig != null && errorTrackingConfig.ActivateTracking)
            {
                var uri = new Uri("http://reporting.onetrueerror.com");
                if (!string.IsNullOrEmpty(errorTrackingConfig.ContactEmail) ||
                    !string.IsNullOrEmpty(errorTrackingConfig.InstallationId))
                {
                    OneTrue.Configuration.ContextProviders.Add(new CustomerInfoProvider(
                        errorTrackingConfig.ContactEmail,
                        errorTrackingConfig.InstallationId));
                }
                OneTrue.Configure(uri, "appKey", "sharedSecret");
            }
            else
            {
                GlobalConfiguration.Configuration.Services.Add(typeof (IExceptionLogger), new WebApiLogger());
            }
        }

        private void ConfigureStandardSetup()
        {
            var provider = new AssemblyScanningMappingProvider();
            provider.Scan(typeof (UserMapper).Assembly);
            EntityMappingProvider.Provider = provider;

            OneTrueErrorPrincipal.Assigned += OnAssignedPrincipal;
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            _serviceRunner.Start();
        }

        private static bool IsWebApiRequest()
        {
            return
                HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(
                    WebApiConfig.UrlPrefixRelative);
        }

        private void OnAssignedPrincipal(object sender, EventArgs e)
        {
            HttpContext.Current.User = (IPrincipal) sender;
        }
    }
}