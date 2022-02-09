using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.App.Core.Accounts;
using Coderr.Server.Common.App;
using Coderr.Server.Common.Data.SqlServer.Schema;
using Coderr.Server.Infrastructure;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.Premise.App;
using Coderr.Server.Premise.App.Schema;
using Coderr.Server.Premise.Boot;
using Coderr.Server.Premise.Boot.Authentication;
using Coderr.Server.SqlServer;
using Coderr.Server.SqlServer.Migrations;
using Coderr.Server.SqlServer.Schema;
using Coderr.Server.Web.Areas.Installation;
using Coderr.Server.Web.Boot;
using Coderr.Server.Web.Boot.Adapters;
using Coderr.Server.Web.Boot.Cqs;
using Coderr.Server.Web.Controllers;
using Coderr.Server.Web.Infrastructure;
using Coderr.Server.Web.Infrastructure.Logging;
using Griffin.Data;
using log4net;
using log4net.Appender;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Coderr.Server.Web
{
    public class Startup
    {
        private readonly AppModuleStarter _appModuleStarter;
        private readonly ILog _logger = LogManager.GetLogger(typeof(Startup));
        private readonly QueueManager _queueManager = new QueueManager();
        private IServiceProvider _serviceProvider;
        private IApplicationLifetime _applicationLifeTime;

        public Startup(IConfiguration configuration)
        {
            ServerConfig.Instance.ServerType = configuration.GetSection("AppSettings").GetValue<bool>("IsLive")
                ? ServerType.Live
                : ServerType.Premise;
#if PREMISE
            ServerConfig.Instance.ServerType = ServerType.Premise;
#endif
            _queueManager.Configure(new ConfigurationWrapper(configuration), OpenConnection);
            if (ServerConfig.Instance.IsLive)
                _queueManager.SetCustomProvider(
                    new AzureQueueProvider(configuration.GetSection("MessageQueue")["ConnectionString"]));
            else
                _queueManager.ShutdownRequested += OnShutdownRequested;

            var editionText = ServerConfig.Instance.IsLive ? "Live" : "Premise";
            _logger.Debug($"Running as {editionText}.");
            Configuration = configuration;
            _appModuleStarter = new AppModuleStarter(configuration);

            LoadHostConfiguration();
            ConfigureMigrations();

            if (ServerConfig.Instance.IsLive || !IsConfigured)
            {
                _logger.Debug("Live, will not use general dbStore");
                return;
            }

            var configStore = new DatabaseStore(() => OpenConnection(CoderrClaims.SystemPrincipal));
            LicenseWrapper.Instance = new LicenseWrapper(configStore);
            LicenseWrapper.Instance.ValidateLicense();
        }

        private void ConfigureMigrations()
        {
            var baseRunner = new MigrationRunner(
                () => OpenConnection(CoderrClaims.SystemPrincipal),
                "Coderr",
                typeof(CoderrMigrationPointer).Namespace);
            SqlServerTools.AddMigrationRunner(baseRunner);



            var commonRunner = new MigrationRunner(
                    () => OpenConnection(CoderrClaims.SystemPrincipal),
                    "Common",
                    typeof(CommonSchemaPointer).Namespace);
            SqlServerTools.AddMigrationRunner(commonRunner);

            if (ServerConfig.Instance.IsLive)
                return;

            var premiseRunner = new MigrationRunner(
                () => OpenConnection(CoderrClaims.SystemPrincipal),
                "Premise",
                typeof(PremiseSchemaPointer).Namespace);
            SqlServerTools.AddMigrationRunner(premiseRunner);
        }

        private void LoadHostConfiguration()
        {
            HostConfig.Instance.IsRunningInDocker =
                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
            if (HostConfig.Instance.IsRunningInDocker)
            {
                HostConfig.Instance.ConfigurationPassword =
                    Environment.GetEnvironmentVariable("CODERR_CONFIG_PASSWORD");
                HostConfig.Instance.IsConfigured = string.IsNullOrEmpty(HostConfig.Instance.ConfigurationPassword);

                HostConfig.Instance.ConnectionString = Environment.GetEnvironmentVariable("CODERR_CONNECTIONSTRING");
                ((log4net.Repository.Hierarchy.Logger)_logger.Logger).AddAppender(new ManagedColoredConsoleAppender());
            }
            else
            {
                var configSection = Configuration.GetSection("Installation");
                HostConfig.Instance.ConfigurationPassword = configSection.GetValue<string>("Password");
                HostConfig.Instance.IsConfigured = configSection.GetValue<bool>("IsConfigured");
                HostConfig.Instance.ConnectionString = Configuration["ConnectionStrings:Db"];
                HostConfig.Instance.IsDemo = Configuration.GetSection("General")?.GetValue<bool>("IsDemo") ?? Debugger.IsAttached;
            }

            Console.WriteLine(HostConfig.Instance);
        }


        public static IConfiguration Configuration { get; private set; }

        protected bool UseWindowsAuthentication
        {
            get
            {
                var settings = Configuration.GetValue<string>("WindowsAuthentication");
                return settings?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
            }
        }

        private bool IsConfigured
        {
            get
            {
                var sesion = Configuration.GetSection("Installation");
                return sesion.GetValue<bool>("IsConfigured");
            }
        }

        /// <summary>
        ///     Invoked after ConfigureServices.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            _applicationLifeTime = applicationLifetime;
            _logger.Info("Launching " + env.EnvironmentName + " from " + env.WebRootPath);

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            ConfigureCoderr(app);

            if (env.IsDevelopment()
                || env.EnvironmentName.IndexOf("dev", StringComparison.OrdinalIgnoreCase) != -1)
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ConfigFile = "webpack.config.dev.js"
                });
            }
            else if (Environment.GetEnvironmentVariable("CustomErrors") == "true") app.UseDeveloperExceptionPage();


            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });


            // OSS
            //app.UseApiKeyMiddleware(() => OpenConnection(CoderrClaims.SystemPrincipal));

            if (ServerConfig.Instance.IsLive)
                app.AddLiveBeforeAuthMiddleware(new ConfigurationWrapper(Configuration));
            else
                app.AddPremiseBeforeAuthMiddleware(new ConfigurationWrapper(Configuration));

            app.UseStaticFiles();
            app.UseMiddleware<PendingRequestTrackingMiddleware>();
            app.UseAuthentication();

            if (ServerConfig.Instance.IsLive)
                app.AddLiveAfterAuthMiddleware(new ConfigurationWrapper(Configuration));
            else
            {
                app.Use(async (context2, next) =>
                {
                    var winId = (WindowsIdentity)context2.User.Identities.FirstOrDefault(x => x is WindowsIdentity);
                    if (winId == null)
                    {
                        await next();
                        return;
                    }

                    var con = context2.RequestServices.GetRequiredService<IAdoNetUnitOfWork>();
                    var accountService = context2.RequestServices.GetRequiredService<IAccountService>();
                    var pLoader = new AdPrincipalBuilder(con, accountService);
                    var user = await pLoader.CreateIdentity(winId);
                    if (user == null)
                    {
                        context2.User = new GenericPrincipal(new GenericIdentity(""), new string[0]);
                        context2.Response.Redirect("/account/login/");
                    }
                    else
                    {
                        var principal = new ClaimsPrincipal(user);
                        var props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.Today.AddDays(14)
                        };

                        await context2.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                            props);
                        context2.User = principal;
                        context2.RequestServices.GetService<IPrincipalAccessor>().Principal = principal;
                    }


                    await next();
                });
            }

            if (UseWindowsAuthentication)
            {
                app.UseStatusCodePages(ctx =>
                {
                    if (ctx.HttpContext.Response.StatusCode == 403)
                        ctx.HttpContext.Response.Redirect("/account/forbidden/");

                    return Task.CompletedTask;
                });
            }
            else
            {
                app.UseStatusCodePages(async x =>
                {
                    _logger.Error($"{x.HttpContext.Response.StatusCode}: {x.HttpContext.Request.GetDisplayUrl()}");
                    x.HttpContext.Response.ContentType = "text/plain";

                    var erroMsg = "General Failure";
                    var exceptionHandlerPathFeature =
                        x.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature != null)
                        erroMsg = exceptionHandlerPathFeature.Error.Message;

                    await x.HttpContext.Response.WriteAsync(
                        "Error " +
                        x.HttpContext.Response.StatusCode + " " + erroMsg);
                });
            }
            //app.Use(async (context2, next) =>
            //    {
            //        var winId = context2.User.Identities.FirstOrDefault(x => x is WindowsIdentity);
            //        await next();
            //    });

            app.UseMvc(routes =>
            {
                if (!IsConfigured)
                {
                    routes.MapRoute(
                        "areas",
                        "{area:exists}/{controller=Setup}/{action=Index}/{id?}",
                        new { area = "Installation" }
                    );
                }

                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute(
                    "spa-fallback",
                    new { controller = "Home", action = "Index" });
            });

            if (!HostConfig.Instance.IsConfigured)
                return;

            _serviceProvider = app.ApplicationServices;
            var context = new StartContext { ServiceProvider = _serviceProvider };
            _appModuleStarter.Start(context);
            _logger.Debug("All started");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (!ServerConfig.Instance.IsLive)
                UpgradeDatabaseSchema();


            if (IsCorsEnabled())
            {
                services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }));
            }
            else
            {
                // Add the policy, but do not allow any origins
                // which means that the policy is effectively denying everything.
                services.AddCors(o => o.AddPolicy("CorsPolicy", builder => { builder.AllowAnyHeader(); }));
            }

            services.AddMvc(options =>
                {
                    AddCoderrToMvc(options);
                    options.Filters.Add<InstallAuthorizationFilter>();
                    options.Filters.Add<TransactionalAttribute2>();
                })
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.ContractResolver = new IncludeNonPublicMembersContractResolver();
                });

            var authenticationBuilder = services.AddAuthentication(options =>
            {
                if (ServerConfig.Instance.IsLive || !UseWindowsAuthentication)
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }
                else
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = IISDefaults.AuthenticationScheme;
                    options.DefaultScheme = "Bajs";
                }
            });


            if (ServerConfig.Instance.IsLive)
            {
                authenticationBuilder =
                    authenticationBuilder.AddLiveAuthentication(new ConfigurationWrapper(Configuration));
            }
            else if (UseWindowsAuthentication)
            {
                _logger.Debug("Adding windows authentication");
                authenticationBuilder.AddWindowsAuthentication("Bajs", options =>
                {
                    //options.TicketDataFormat = new SecureDataFormat<AuthenticationTicket>(new MyTicketSerializer(), authenticationBuilder.);
                });
            }

            //authenticationBuilder.AddScheme<MyOptions>("ApiKey", options => { });
            authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //options.LoginPath = "/lobby/visit/";
                //options.LogoutPath= "/lobby/visit/";
                //options.Events.OnRedirectToAccessDenied = context => { return Task.CompletedTask; };
                //options.Events.OnRedirectToLogin = context => { return Task.CompletedTask; };
                //options.Events.OnValidatePrincipal = context =>
                //{
                //    var principal = context.Principal;
                //    var str = context.Request.GetDisplayUrl() + ": " + context.Principal.Identity.IsAuthenticated;
                //    return Task.CompletedTask;
                //};
            });

            if (!HostConfig.Instance.IsConfigured)
            {
                RegisterInstallationConfiguration(services);
                return;
            }

            _appModuleStarter.ScanAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            services.AddSingleton<Abstractions.Boot.IConfiguration>(new ConfigurationWrapper(Configuration));
            services.AddScoped<IPrincipalAccessor, PrincipalWrapper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterConfigurationStores(services);

            var configContext = new CqsObjectMapperConfigurationContext(CqsController._cqsObjectMapper)
            {
                Configuration = new ConfigurationWrapper(Configuration),
                ConnectionFactory = OpenConnection,
                Services = services,
                ServiceProvider = () => _serviceProvider
            };
            _appModuleStarter.Configure(configContext);
            _logger.Debug("All services is now running.");
        }

        private static bool IsCorsEnabled()
        {
            return Configuration["EnableCors"]?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
        }

        private void AddCoderrToMvc(MvcOptions options)
        {
            options.CatchMvcExceptions();
        }

        private void ConfigureCoderr(IApplicationBuilder app)
        {
            var url = new Uri("https://report.coderr.io/");

            /* Community server.
            Err.Configuration.Credentials(url,
                "2b3002d3ab3e4a57ad45cff2210221ab",
                "f381a5c9797f49bd8a3238b892d02806");
            */

            if (ServerConfig.Instance.IsLive)
            {
                Err.Configuration.Credentials(url,
                    "4df2a64611e640c48504562a0ea7084f",
                    "bfaea86f32524f7bb4eb1c7c7c3167b6");
            }
            else
            {
                // Premise
                Err.Configuration.Credentials(url,
                    "2b6abceb722d4fcfb7a74f75832955b7",
                    "fa0d654fda7843ceba058b93c5ad5278");
            }

            Err.Configuration.ThrowExceptions = false;
            Err.Configuration.QueueReports = true;
            app.CatchMiddlewareExceptions();


            if (!HostConfig.Instance.IsConfigured)
                return;

            CoderrConfigSection config;

            try
            {
                var dbStore = new DatabaseStore(() => OpenConnection(CoderrClaims.SystemPrincipal));
                config = dbStore.Load<CoderrConfigSection>();
                if (config == null)
                    return;
            }
            catch (SqlException)
            {
                // We have not yet configured coderr. 
                return;
            }


            // partitions allow us to see the number of affected installations
            Err.Configuration.AddPartition(x => x.AddPartition("InstallationId", config.InstallationId));

            Err.Configuration.FilterCollection.Add(new BundleSlowReportPosts());

            if (string.IsNullOrWhiteSpace(config.ContactEmail))
                return;

            // Allows us to notify you once the error is corrected.
            Err.Configuration.ReportPreProcessor = report =>
            {
                if (string.IsNullOrWhiteSpace(config.ContactEmail))
                    return;

                var collection = CollectionBuilder.Feedback(config.ContactEmail, null);
                var collections = new List<ContextCollectionDTO>(report.ContextCollections.Length + 1);
                collections.AddRange(report.ContextCollections);
                collections.Add(collection);
            };
        }

        private Task HandleAuthenticationFailure(RedirectContext<CookieAuthenticationOptions> ctx)
        {
            if (ctx.Request.GetDisplayUrl().Contains("/api/"))
                ctx.Response.StatusCode = 401;
            else
                ctx.Response.StatusCode = 302;

            ctx.Response.Headers["Location"] = "/account/login/";
            return Task.CompletedTask;
        }

        private void OnShutdown()
        {
            _logger.Info("Application is shutting down");
            try
            {
                _appModuleStarter.Stop();
                QueueManager.Instance.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to shutdown", ex);
            }

            _logger.Info("Application shutdown completed.");
        }

        /// <summary>
        ///     Another process (through the disk queue system) has requested us to shutdown so that it can take over the queue
        ///     files.
        /// </summary>
        /// <returns></returns>
        private void OnShutdownRequested(object sender, ShuttingDownEventArgs shuttingDownEventArgs)
        {
            //too early in the startup process.
            if (_applicationLifeTime == null)
                return;

            shuttingDownEventArgs.CanShutdown = PendingRequestTrackingMiddleware.NumberOfRequests == 0;
            if (shuttingDownEventArgs.CanShutdown)
            {
                _logger.Info("Queue system requested shutdown.");
                _applicationLifeTime.StopApplication();
            }
        }


        private IDbConnection OpenConnection(ClaimsPrincipal arg)
        {
            if (ServerConfig.Instance.IsLive)
                return LiveStart.OpenConnection(arg, new ConfigurationWrapper(Configuration));

            var db = Configuration.GetConnectionString("Db");
            var con = new SqlConnection(db);
            con.Open();
            return con;
        }

        private void RegisterConfigurationStores(IServiceCollection services)
        {
            services.AddTransient(typeof(IConfiguration<>), typeof(ConfigWrapper<>));

            if (!ServerConfig.Instance.IsLive)
            {
                services.AddTransient<ConfigurationStore>(x =>
                    new DatabaseStore(() => OpenConnection(CoderrClaims.SystemPrincipal)));
            }
        }

        private void RegisterInstallationConfiguration(IServiceCollection services)
        {
            SetupTools.DbTools = new SqlServerTools(() => OpenConnection(CoderrClaims.SystemPrincipal));
            var store = new DatabaseStore(() => OpenConnection(CoderrClaims.SystemPrincipal));
            services.AddSingleton<ConfigurationStore>(store);
        }

        private void UpgradeDatabaseSchema()
        {
            // Dont run for new installations
            if (!IsConfigured)
                return;

            try
            {
                var tools = new SqlServerTools(() => OpenConnection(CoderrClaims.SystemPrincipal));
                tools.CreateTables();
            }
            catch (Exception ex)
            {
                _logger.Fatal("DB Migration failed.", ex);
                Err.Report(ex, new { Migration = true });
            }
        }
    }

    internal class LoggingUploader : IReportUploader
    {
        public void UploadFeedback(FeedbackDTO feedback)
        {
            throw new NotImplementedException();
        }

        public void UploadReport(ErrorReportDTO report)
        {
            UploadFailed?.Invoke(this, new UploadReportFailedEventArgs(null, null));
        }

        public event EventHandler<UploadReportFailedEventArgs> UploadFailed;
    }
}