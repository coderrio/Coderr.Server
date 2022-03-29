using System;
using Coderr.Server.Abstractions;
using Coderr.Server.Infrastructure;
using Coderr.Server.SqlServer;
using Coderr.Server.WebSite.Areas.Installation;
using Coderr.Server.WebSite.Infrastructure;
using Coderr.Server.WebSite.Infrastructure.Adapters;
using Coderr.Server.WebSite.Infrastructure.Filters;
using Coderr.Server.WebSite.Infrastructure.WebSockets;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebSocketHub = Coderr.Server.WebSite.Hubs.WebSocketHub;

namespace Coderr.Server.WebSite
{
    public class Startup
    {
        private readonly CoderrStartup _coderrStartup;
        private readonly ILog _logger = LogManager.GetLogger(typeof(Startup));


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _coderrStartup = new CoderrStartup(configuration);
            ServerConfig.Instance.Queues.Configure(new ConfigurationWrapper(configuration));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.Debug("ConfigureServices..");
            _coderrStartup.BeginConfigureServices(services);

            EnableCors(services);

            var authBuilder = services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtHelper.Configure);
            _coderrStartup.AddAuthentication(authBuilder);

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<InstallAuthorizationFilter>();
                options.Filters.Add<TransactionalAttribute>();
            });
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            _coderrStartup.EndConfigureServices(services);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        bool IsCorsEnabled => Configuration["EnableCors"]?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;  

        public bool IsDevelopment(IWebHostEnvironment env)
        {
            return env.IsDevelopment() || env.EnvironmentName == "onpremisedev";
        }

        private void EnableCors(IServiceCollection services)
        {
            if (IsCorsEnabled)
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                _coderrStartup.Start();
            });

            _logger.Debug("Configure..");
            _coderrStartup.Configure(app, applicationLifetime);

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };
            app.UseWebSockets(webSocketOptions);

            if (IsCorsEnabled)
            {
                app.UseCors("CorsPolicy");
            }

            MapWebManifest(app);

            if (IsDevelopment(env))
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMiddleware<PendingRequestTrackingMiddleware>();
            app.UseForwardedHeaders();

            if (!IsDevelopment(env))
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            _coderrStartup.ConfigureAuthentication(app);


            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path != "/ws")
            //    {
            //        await next();
            //        return;
            //    }

            //    if (!context.WebSockets.IsWebSocketRequest)
            //    {
            //        context.Response.StatusCode = 400;
            //        return;
            //    }

            //    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //    var service = context.RequestServices.GetRequiredService<IWebSocketHub>();
            //    await service.Process(context.RequestAborted, webSocket, context.User);
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Installation",
                    pattern: "{area:exists}/{controller=Setup}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<WebSocketHub>("/hub");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (IsDevelopment(env))
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            _coderrStartup.ConfigureEnd(app);
        }

        private static void MapWebManifest(IApplicationBuilder app)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
        }
    }
}
