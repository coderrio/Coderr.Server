using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using Coderr.Client;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Infrastructure;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.ReportAnalyzer.Boot.Adapters;
using Coderr.Server.SqlServer;
using Coderr.Server.SqlServer.Migrations;
using Coderr.Server.SqlServer.Schema;
using Coderr.Server.SqlServer.Schema.Common;
using Coderr.Server.SqlServer.Schema.Oss;
using Coderr.Server.WebSite.Controllers;
using Coderr.Server.WebSite.Infrastructure.Adapters;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using RegisterExtensions = Coderr.Server.Abstractions.Boot.RegisterExtensions;

namespace Coderr.Server.WebSite.Infrastructure
{
    public class CoderrStartup
    {
        private readonly IConfiguration _configuration;
        private readonly ILog _logger = LogManager.GetLogger(typeof(Startup));
        private readonly QueueManager _queueManager = new QueueManager();
        private IHostApplicationLifetime _applicationLifetime;
        private readonly AppModuleStarter _appModuleStarter;
        private IServiceProvider _serviceProvider;
        
        public CoderrStartup(IConfiguration configuration)
        {
            _configuration = configuration;
            ServerConfig.Instance.ServerType = _configuration.GetSection("AppSettings").GetValue<bool>("IsLive")
                ? ServerType.Live
                : ServerType.Premise;
            HostConfig.Instance.InstallationPassword = _configuration["General:InstallationPassword"];

            var configWrapper = new ConfigurationWrapper(_configuration);

            LoadHostConfiguration(_configuration);

            _appModuleStarter = new AppModuleStarter(_configuration);
            _queueManager.Configure(configWrapper, OpenConnection);
            _queueManager.ShutdownRequested += OnShutdownRequested;

            ValidateConfiguration(configuration);
            ConfigureMigrations();
        }

        private void ValidateConfiguration(IConfiguration configuration)
        {
            var tools = new SqlServerTools(() => OpenConnection(CoderrClaims.SystemPrincipal));
            SetupTools.DbTools = tools;
            var conString = configuration.GetConnectionString("Db");
            if (!string.IsNullOrEmpty(conString) && !conString.Contains("ReplaceMe") && tools.IsConfigurationComplete(conString))
            {
                HostConfig.Instance.MarkAsConfigured();
            }
        }

        public void Configure(IApplicationBuilder applicationBuilder, IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        public void ConfigureAfterAuthentication(IApplicationBuilder app)
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

        public void ConfigureEnd(IApplicationBuilder applicationBuilder)
        {
            _serviceProvider = applicationBuilder.ApplicationServices;
            HostConfig.Instance.Configured += (sender, args) =>
            {
                var context = new StartContext { ServiceProvider = applicationBuilder.ApplicationServices };
                _appModuleStarter.Start(context);
                _logger.Info("Coderr started successfully.");
            };
        }

        public void BeginConfigureServices(IServiceCollection services)
        {
            if (!ServerConfig.Instance.IsLive)
                UpgradeDatabaseSchema();
        }

        public void EndConfigureServices(IServiceCollection services)
        {
            _appModuleStarter.ScanAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            services.AddSingleton<Abstractions.Boot.IConfiguration>(new ConfigurationWrapper(_configuration));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IAdoNetUnitOfWork, AdoNetUnitOfWork>();

            RegisterExtensions.RegisterContainerServices(services, Assembly.GetExecutingAssembly());
            //Coderr.Server.WebSite.Infrastructure.Modules.RegisterContainerServices()
            //services.RegisterContainerServices(Assembly.GetExecutingAssembly());

            RegisterConfigurationStores(services);

            var configContext = new CqsObjectMapperConfigurationContext(CqsController._cqsObjectMapper, services, () => _serviceProvider)
            {
                Configuration = new ConfigurationWrapper(_configuration),
                ConnectionFactory = OpenConnection
            };
            _appModuleStarter.Configure(configContext);
            var k = services.Where(x => x.ServiceType.Name == "ITriggerRepository").ToList();
            _logger.Debug("All services is now running.");
        }

        private void ConfigureMigrations()
        {
            var baseRunner = new MigrationRunner(
                () => OpenConnection(CoderrClaims.SystemPrincipal),
                "Coderr",
                typeof(CoderrMigrationPointer).Namespace);
            var commonRunner = new MigrationRunner(
                () => OpenConnection(CoderrClaims.SystemPrincipal),
                "Common",
                typeof(CommonSchemaPointer).Namespace);
            SqlServerTools.AddMigrationRunner(baseRunner);
            SqlServerTools.AddMigrationRunner(commonRunner);
        }

        private void LoadHostConfiguration(IConfiguration configuration)
        {
            HostConfig.Instance.IsRunningInDocker =
                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
            if (HostConfig.Instance.IsRunningInDocker)
            {
                HostConfig.Instance.ConnectionString = Environment.GetEnvironmentVariable("CODERR_CONNECTIONSTRING");

                ((Logger)_logger.Logger).AddAppender(new ManagedColoredConsoleAppender());
            }
            else
            {
                HostConfig.Instance.ConnectionString = configuration["ConnectionStrings:Db"];
                HostConfig.Instance.IsDemo = configuration.GetSection("General")?.GetValue<bool>("IsDemo") ??
                                             Debugger.IsAttached;
            }

            Console.WriteLine(HostConfig.Instance);
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

        private void OnShutdownRequested(object sender, ShuttingDownEventArgs shuttingDownEventArgs)
        {
            //too early in the startup process.
            if (_applicationLifetime == null)
                return;

            shuttingDownEventArgs.CanShutdown = PendingRequestTrackingMiddleware.NumberOfRequests == 0;
            if (!shuttingDownEventArgs.CanShutdown) return;

            _logger.Info("Queue system requested shutdown.");
            _applicationLifetime.StopApplication();
        }

        private IDbConnection OpenConnection(ClaimsPrincipal arg)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Db"));
            try
            {
                connection.Open();
            }
            catch (SqlException)
            {
                Thread.Sleep(500);
                connection.Open();
            }

            return connection;
        }

        private void RegisterConfigurationStores(IServiceCollection services)
        {
            services.AddTransient(typeof(IConfiguration<>), typeof(ConfigWrapper<>));

            if (!ServerConfig.Instance.IsLive)
                services.AddTransient<ConfigurationStore>(x =>
                    new DatabaseStore(() => OpenConnection(CoderrClaims.SystemPrincipal)));
        }

        private void UpgradeDatabaseSchema()
        {
            // Don't run for new installations
            if (!HostConfig.Instance.IsConfigured)
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

        public void Start()
        {
            if (HostConfig.Instance.IsConfigured)
            {
                HostConfig.Instance.TriggeredConfigured();
            }
        }
    }
}