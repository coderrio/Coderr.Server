using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Infrastructure;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.SqlServer;
using Coderr.Server.Web.Areas.Installation;
using Coderr.Server.Web.Boot;
using Coderr.Server.Web.Boot.Adapters;
using Coderr.Server.Web.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Coderr.Server.Web
{
    public class Startup
    {
        private readonly ModuleStarter _moduleStarter;
        private IServiceProvider _serviceProvider;


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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                if (!IsConfigured)
                {
                    routes.MapRoute(
                        "areas",
                        "{area:exists}/{controller=Setup}/{action=Index}/{id?}",
                        defaults: new { area = "Installation" }
                    );
                }

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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => { options.Filters.Add<InstallAuthorizationFilter>(); })
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.ContractResolver = new IncludeNonPublicMembersContractResolver();
                });

            var authenticationBuilder = services.AddAuthentication("Cookies");
            authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //options.ExpireTimeSpan = TimeSpan.FromDays(14);
                //options.p
                //options.Events.OnRedirectToLogin = HandleAuthenticationFailure;
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

        private IServiceProvider GetLazyLoadedServiceProvider()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("It's not built yet.");

            return _serviceProvider;
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