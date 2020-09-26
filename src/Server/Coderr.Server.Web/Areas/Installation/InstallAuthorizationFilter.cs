using Coderr.Server.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Coderr.Server.Web.Areas.Installation
{
    /// <summary>
    ///     Deny access to installation wizard once the application is configured
    /// </summary>
    public class InstallAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public InstallAuthorizationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static bool IsInstallationCompleted { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (IsInstallationCompleted)
            {
                ValidatePostInstallation(context);
                return;
            }

            if (!context.HttpContext.Request.Path.Value.Contains("/installation/"))
                return;

            var section = _configuration.GetSection("Installation");
            if (section == null)
                return;

            if (!HostConfig.Instance.IsConfigured) return;

            context.Result = new ContentResult
            {
                StatusCode = 403,
                Content = "The installation wizard has been disabled. Goto the root of the website.",
                ContentType = "text/plain"
            };
        }

        private void ValidatePostInstallation(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Path.Value.Contains("/installation/"))
                return;

            context.Result = new ContentResult
            {
                StatusCode = 403,
                Content =
                    "Configuration wizard have been active. You must therefore restart the application pool so that all background services can start properly.",
                ContentType = "text/plain"
            };
        }
    }
}