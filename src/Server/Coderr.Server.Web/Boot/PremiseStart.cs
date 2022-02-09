using System;
using System.Data;
using System.Data.SqlClient;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Premise.Boot.Authentication;
using Coderr.Server.Premise.Boot.Authentication.ApiKeys;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;

namespace Coderr.Server.Web.Boot
{
    public static class PremiseStart
    {
        private static ILog _logger = LogManager.GetLogger(typeof(PremiseStart));
        public static void AddPremiseBeforeAuthMiddleware(this IApplicationBuilder app, IConfiguration configuration)
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
                    OpenDb = () => OpenConnection(configuration),
                    AuthenticationScheme = "ApiKey",
                };

                try
                {
                    var ticket = await ApiKeyAuthenticator.CreateTicket(context2.Request, options);
                    if (ticket == null)
                    {
                        _logger.Warn("Unauthorized");
                        await context2.ForbidAsync();
                        return;
                    }

                    await context2.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ticket.Principal);
                    context2.User = ticket.Principal;
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to authenticate", ex);
                }


                await next.Invoke();
            });

        }

        private static IDbConnection OpenConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Db");
            if (connectionString == null)
                throw new InvalidOperationException("Failed to find connection string 'Db'.");

            var con = new SqlConnection(connectionString);
            con.Open();
            return con;
            //return con.ToCoderrConnection();
        }
    }
}
