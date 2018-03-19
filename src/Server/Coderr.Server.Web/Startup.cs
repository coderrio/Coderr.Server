using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Infrastructure.Boot;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.PluginApi.Config;
using Coderr.Server.Web2.Boot;
using Coderr.Server.Web2.Boot.Adapters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Coderr.Server.Web2
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
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute(
                    "spa-fallback",
                    new { controller = "Home", action = "Index" });
            });

            _serviceProvider = app.ApplicationServices;
            var context = new StartContext
            {
                ServiceProvider = app.ApplicationServices
            };
            _moduleStarter.Start(context);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                
            }).AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.ContractResolver = new IncludeNonPublicMembersContractResolver();
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
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

            _moduleStarter.ScanAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            RegisterConfigurationStores(services);

            var configContext = new ConfigurationContext
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
            {
                ctx.Response.StatusCode = 401;
            }
            else
            {
                ctx.Response.StatusCode = 302;
            }

            ctx.Response.Headers["Location"] = "/account/login/";
            return Task.CompletedTask;
        }

        private IDbConnection OpenConnection(ClaimsPrincipal arg)
        {
            return DbConnectionConfig.OpenConnection();
        }

        private void RegisterConfigurationStores(IServiceCollection services)
        {
            services.AddTransient(typeof(IConfiguration<>), typeof(ConfigWrapper<>));
            services.AddTransient<ConfigurationStore>(x => new DatabaseStore(DbConnectionConfig.OpenConnection));
        }
    }
}