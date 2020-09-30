using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Infrastructure;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.SqlServer;
using Coderr.Server.SqlServer.Migrations;
using Coderr.Server.SqlServer.Schema;
using Coderr.Server.Web.Areas.Installation;
using Coderr.Server.Web.Boot;
using Coderr.Server.Web.Boot.Adapters;
using Coderr.Server.Web.Controllers;
using Coderr.Server.Web.Infrastructure;
using Coderr.Server.Web.Infrastructure.Authentication.ApiKeys;
using log4net;
using log4net.Appender;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using IncludeNonPublicMembersContractResolver = Coderr.Server.Infrastructure.Messaging.IncludeNonPublicMembersContractResolver;

namespace Coderr.Server.Web
{
    public class Startup
    {
        private readonly AppModuleStarter _appModuleStarter;
        private IServiceProvider _serviceProvider;
        private ILog _logger = LogManager.GetLogger(typeof(Startup));

        public Startup(IConfiguration configuration)
        {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddConfiguration(configuration)
                .Build();

            _appModuleStarter = new AppModuleStarter(Configuration);


            LoadHostConfiguration();
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
            }

            Console.WriteLine(HostConfig.Instance);
        }

        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Invoked after ConfigureServices.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="applicationLifetime"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            ConfigureCoderr(app);
            UpgradeDatabaseSchema();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ConfigFile = "webpack.config.dev.js",
                });
            }


            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider
            });


            app.UseApiKeyMiddleware(() => OpenConnection(CoderrClaims.SystemPrincipal));
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                if (!HostConfig.Instance.IsConfigured)
                    routes.MapRoute(
                        "areas",
                        "{area:exists}/{controller=Setup}/{action=Index}/{id?}",
                        new { area = "Installation" }
                    );

                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute(
                    "spa-fallback",
                    new
                    {
                        controller = "Home",
                        action = "Index"
                    });
            });

            if (!HostConfig.Instance.IsConfigured)
                return;

            _serviceProvider = app.ApplicationServices;
            var context = new StartContext
            {
                ServiceProvider = app.ApplicationServices
            };
            _appModuleStarter.Start(context);
        }

        private void UpgradeDatabaseSchema()
        {
            // Dont run for new installations
            if (!HostConfig.Instance.IsConfigured)
                return;

            try
            {
                var migrator = new MigrationRunner(() => OpenConnection(CoderrClaims.SystemPrincipal), "Coderr", typeof(CoderrMigrationPointer).Namespace);
                migrator.Run();
            }
            catch (Exception ex)
            {
                _logger.Fatal("DB Migration failed.", ex);
                Err.Report(ex, new { Migration = true });
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Configuration["EnableCors"]?.Equals("true", StringComparison.OrdinalIgnoreCase) == true)
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
                services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader();
                }));
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


            var authenticationBuilder = services.AddAuthentication("Cookies");
            authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnValidatePrincipal = context =>
                {
                    var principal = context.Principal;
                    var str = context.Request.GetDisplayUrl() + ": " + context.Principal.Identity.IsAuthenticated;
                    return Task.CompletedTask;
                };
            });
            authenticationBuilder.AddScheme<ApiKeyAuthOptions, ApiKeyAuthenticator>(ApiKeyAuthOptions.DefaultSchemeName, "ApiKey authentication",
                options =>
                {
                    options.OpenDb = () => OpenConnection(CoderrClaims.SystemPrincipal);
                    options.AuthenticationScheme = ApiKeyAuthOptions.DefaultSchemeName;
                });

            if (!HostConfig.Instance.IsConfigured)
            {
                RegisterInstallationConfiguration(services);
                return;
            }

            _appModuleStarter.ScanAssemblies(AppDomain.CurrentDomain.BaseDirectory);

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
        }

        private void AddCoderrToMvc(MvcOptions options)
        {
            options.CatchMvcExceptions();
        }

        private void ConfigureCoderr(IApplicationBuilder app)
        {
            var url = new Uri("https://report.coderr.io/");
            Err.Configuration.Credentials(url,
                "2b3002d3ab3e4a57ad45cff2210221ab",
                "f381a5c9797f49bd8a3238b892d02806");
            Err.Configuration.ThrowExceptions = false;
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
            _appModuleStarter.Stop();
        }

        private IDbConnection OpenConnection(ClaimsPrincipal arg)
        {
            var con = new SqlConnection(HostConfig.Instance.ConnectionString);
            con.Open();
            return con;
        }

        private void RegisterConfigurationStores(IServiceCollection services)
        {
            services.AddTransient(typeof(IConfiguration<>), typeof(ConfigWrapper<>));
            services.AddTransient<ConfigurationStore>(x =>
                new DatabaseStore(() => OpenConnection(CoderrClaims.SystemPrincipal)));
        }

        private void RegisterInstallationConfiguration(IServiceCollection services)
        {
            SetupTools.DbTools = new SqlServerTools(() => OpenConnection(CoderrClaims.SystemPrincipal));
            var store = new DatabaseStore(() => OpenConnection(CoderrClaims.SystemPrincipal));
            services.AddSingleton<ConfigurationStore>(store);
        }
    }
}