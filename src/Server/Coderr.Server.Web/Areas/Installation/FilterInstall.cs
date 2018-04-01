using System;
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

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Path.Value.Contains("/installation/"))
                return;

            var section = _configuration.GetSection("Installation");
            if (section == null)
                return;

            var isConfigured = section.GetValue<bool>("IsConfigured");
            if (!isConfigured)
                return;

            context.Result = new ContentResult
            {
                StatusCode = 403,
                Content = "Installation wizard have been disabled",
                ContentType = "text/plain"
            };
        }
    }
}