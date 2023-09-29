using System;
using System.Data;
using System.Data.SqlClient;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Infrastructure.Configuration.Database;
using Coderr.Server.SqlServer;
using Coderr.Server.SqlServer.Migrations;
using Coderr.Server.WebSite.Infrastructure.ApiKeyAuthentication;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;

namespace Coderr.Server.WebSite.Infrastructure.Premise
{
    public static class PremiseStart
    {
        private static ILog _logger = LogManager.GetLogger(typeof(PremiseStart));
        private static IConfiguration _config;

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

        public static IDbConnection OpenConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Db");
            if (connectionString == null)
                throw new InvalidOperationException("Failed to find connection string 'Db'.");

            var con = new SqlConnection(connectionString);
            con.Open();
            return con;
            //return con.ToCoderrConnection();
        }

        public static bool UseWindowsAuthentication
        {
            get
            {
                var settings = _config.GetSection("Premise")["WindowsAuthentication"];
                return settings?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
            }
        }

        public static void Init(IConfiguration config)
        {
            _config = config;
            var configStore = new DatabaseStore(() => OpenConnection(config));
            HostConfig.Instance.Configured += (sender, args) =>
            {
            };

            //var premiseRunner = new MigrationRunner(
            //    () => OpenConnection(config),
            //    "Premise",
            //    typeof(PremiseSchemaPointer).Namespace);
            //SqlServerTools.AddMigrationRunner(premiseRunner);
        }
    }
}
