using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Client.AspNetCore.Mvc;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.App.Core.Reports.Config;
using Coderr.Server.Infrastructure;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.SqlServer;
using Coderr.Server.SqlServer.Migrations;
using Coderr.Server.Web.Areas.Installation;
using Coderr.Server.Web.Boot;
using Coderr.Server.Web.Boot.Adapters;
using Coderr.Server.Web.Controllers;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Coderr.Server.Web
{
    public class Startup
    {
        private readonly ModuleStarter _moduleStarter;
        private IServiceProvider _serviceProvider;
        private ILog _logger = LogManager.GetLogger(typeof(Startup));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _moduleStarter = new ModuleStarter(configuration);
        }

        public IConfiguration Configuration { get; }

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
                    HotModuleReplacement = true
                });
            }


            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                if (!IsConfigured)
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

            if (!IsConfigured)
                return;

            _serviceProvider = app.ApplicationServices;
            var context = new StartContext
            {
                ServiceProvider = app.ApplicationServices
            };
            _moduleStarter.Start(context);
        }

        private void UpgradeDatabaseSchema()
        {
            // Dont run for new installations
            if (!IsConfigured) 
                return;

            try
            {
                var migrator = new SchemaManager(() => OpenConnection(CoderrClaims.SystemPrincipal));
                if (migrator.CanSchemaBeUpgraded())
                {
                    migrator.UpgradeDatabaseSchema();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DB Migration failed.", ex);
                Err.Report(ex, new {Migration = true});
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    AddCoderrToMvc(options);
                    options.Filters.Add<InstallAuthorizationFilter>();
                })
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.ContractResolver = new IncludeNonPublicMembersContractResolver();
                });

            if (Configuration["EnableCors"] == "true")
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
                // which means that the policy is effectivly denying everything.
                services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader();
                }));
            }

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

            if (!IsConfigured)
            {
                RegisterInstallationConfiguration(services);
                return;
            }

            _moduleStarter.ScanAssemblies(AppDomain.CurrentDomain.BaseDirectory);

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
            _moduleStarter.Configure(configContext);
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
            app.CatchOwinExceptions();


            if (!IsConfigured)
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
            _moduleStarter.Stop();
        }

        private IDbConnection OpenConnection(ClaimsPrincipal arg)
        {
            var db = Configuration.GetConnectionString("Db");
            var con = new SqlConnection(db);
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
            services.Configure<InstallationOptions>(Configuration.GetSection("Installation"));
        }
    }
}