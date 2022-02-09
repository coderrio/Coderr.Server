using System;
using System.Data;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading;
using Coderr.Client;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Live.Abstractions;
using Coderr.Server.Live.Boot;
using Coderr.Server.Live.LobbyIntegration.Authentication;
using Coderr.Server.Live.LobbyIntegration.Authentication.ApiKeys;
using Coderr.Server.Live.LobbyIntegration.Authentication.SingleSignOn;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Web.Boot
{
    public static class LiveStart
    {
        private static ILog _logger = LogManager.GetLogger(typeof(LiveStart));
        private static Timer _timer = new Timer(OnCheckConnections, null, 60000, 60000);
        private static int _lastConnectionCount;

        private static void OnCheckConnections(object state)
        {
            try
            {
                //var cons = Client.AdoNet.CoderrAdoNet.ActiveConnections.ToList();
                //if (_lastConnectionCount == cons.Count)
                //    return;


                //_lastConnectionCount = cons.Count;
                //_logger.Debug("New connection count: " + cons.Count);
                //foreach (var connectionInfo in cons)
                //{
                //    _logger.Debug(
                //        $"{connectionInfo.ConnectionString} Last command: {connectionInfo.Commands.LastOrDefault()}");
                //}
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check connections", ex);
            }
        }

        public static void AddLiveAfterAuthMiddleware(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Use(async (context2, next) =>
            {
                if (context2.User.Identity.IsAuthenticated)
                    context2.RequestServices.GetService<IPrincipalAccessor>().Principal = context2.User;
                await next.Invoke();
            });
        }

        public static void AddLiveBeforeAuthMiddleware(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Use(async (context2, next) =>
            {
                if (context2.Request.Query.ContainsKey("s"))
                {
                    var options = new SingleSignOnAuthenticationOptions
                    {
                        OpenGlobalDb = () => OpenGlobalConnection(configuration),
                        OpenTenantDb = orgId => OpenTenantDb(configuration, orgId),
                        AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme
                    };
                    var ticket =
                        await SingleSignOnAuthenticationHandler.CreateAuthenticationTicket(context2.Request, options);
                    if (ticket != null)
                    {
                        await context2.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ticket.Principal, ticket.Properties);
                        context2.User = ticket.Principal;
                        context2.RequestServices.GetService<IPrincipalAccessor>().Principal = ticket.Principal;

                        if (ticket.Properties.RedirectUri != null)
                        {
                            context2.Response.Redirect(ticket.Properties.RedirectUri);
                            return;
                        }

                    }

                    await context2.ForbidAsync();
                    return;
                }

                if (context2.Request.Headers["X-Api-Key"].Count == 1)
                {
                    var options = new ApiKeyAuthOptions
                    {
                        OpenGlobalDb = () => OpenGlobalConnection(configuration),
                        OpenTenantDb = orgId => OpenTenantDb(configuration, orgId),
                        AuthenticationScheme = "ApiKey",
                    };
                    var ticket = await ApiKeyAuthenticator.CreateTicket(context2.Request, options);
                    if (ticket == null)
                    {
                        await context2.ForbidAsync();
                        return;
                    }
                    await context2.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ticket.Principal);
                    context2.User = ticket.Principal;
                }

                try
                {
                    await next.Invoke();
                }
                catch (SecurityException exception)
                {
                    Err.Report(exception);
                    await context2.SignOutAsync();
                    context2.Response.Redirect("/");
                }
            });
        }

        public static IDbConnection OpenConnection(ClaimsPrincipal claimsPrincipal, IConfiguration configuration)
        {
            if (claimsPrincipal.Identity?.IsAuthenticated != true)
                return OpenGlobalConnection(configuration);

            var orgId = claimsPrincipal.GetOrganizationId();
            return orgId == 0
                ? OpenGlobalConnection(configuration)
                : OpenTenantDb(configuration, orgId);
        }

        internal static AuthenticationBuilder AddLiveAuthentication(this AuthenticationBuilder authenticationBuilder,
            IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return authenticationBuilder.AddApiKeyAuthentication(ApiKeyAuthOptions.DefaultSchemeName, options =>
            {
                options.OpenGlobalDb = () => OpenGlobalConnection(configuration);
                options.OpenTenantDb = orgId => OpenTenantDb(configuration, orgId);
                options.AuthenticationScheme = ApiKeyAuthOptions.DefaultSchemeName;
            })
                .AddSingleSignOn("SingleSignOn", options =>
            {
                options.OpenGlobalDb = () => OpenGlobalConnection(configuration);
                options.OpenTenantDb = orgId => OpenTenantDb(configuration, orgId);
                options.AuthenticationScheme = "Cookies";
            });
        }

        public static IDbConnection OpenGlobalConnection(IConfiguration configuration)
        {
            return LiveConnectionFactory.OpenGlobalConnection(configuration);
        }

        public static IDbConnection OpenTenantDb(IConfiguration configuration, int organizationId)
        {
            return LiveConnectionFactory.OpenTenantDb(configuration, organizationId);
        }
    }
}