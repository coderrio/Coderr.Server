using Coderr.Server.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Coderr.Server.WebSite.Areas.Installation
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

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Path.Value.Contains("/installation"))
                return;

            if (!HostConfig.Instance.IsConfigured) 
                return;

            context.Result = new ContentResult
            {
                StatusCode = 403,
                Content = "The installation wizard has been disabled. Goto the root of the website.",
                ContentType = "text/plain"
            };
        }
    }
}