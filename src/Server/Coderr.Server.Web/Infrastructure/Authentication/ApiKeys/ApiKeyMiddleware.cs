using System;
using System.Data.Common;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;

namespace Coderr.Server.Web.Infrastructure.Authentication.ApiKeys
{
    public static class ApiKeyMiddleware
    {
        private static ILog _logger = LogManager.GetLogger(typeof(ApiKeyMiddleware));

        public static void UseApiKeyMiddleware(this IApplicationBuilder app, Func<System.Data.IDbConnection> connectionFactory)
        {
            app.Use(async (context2, next) =>
            {
                if (context2.Request.Headers["X-Api-Key"].Count < 1)
                {
                    await next.Invoke();
                    return;
                }

                var options = new ApiKeyAuthOptions
                {
                    OpenDb = connectionFactory,
                    AuthenticationScheme = "ApiKey",
                };

                try
                {
                    var ticket = await ApiKeyAuthenticator.CreateTicket(context2.Request, options);
                    if (ticket == null)
                    {
                        _logger.Warn("Unauthorized: " + context2.Request.Headers["X-Api-Key"]);
                        await context2.ForbidAsync();
                        return;
                    }

                    await context2.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ticket.Principal);
                    context2.User = ticket.Principal;
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to authenticate: " + context2.Request.Headers["X-Api-Key"], ex);
                }


                await next.Invoke();
            });

        }
    }
}
