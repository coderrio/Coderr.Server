using System;
using System.Security.Claims;
using Coderr.Server.Abstractions.Security;
using DotNetCqs.Logging;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Web.Boot.Adapters
{
    public class PrincipalWrapper : IPrincipalAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal _principal;
        private ILog _logger = LogManager.GetLogger(typeof(PrincipalWrapper));

        public PrincipalWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal Principal
        {
            get
            {
                if (_principal != null)
                {
                    return _principal;
                }

                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("Principal was not assigned and the HttpContext is not available.");

                return _httpContextAccessor.HttpContext.User;
            }

            set => _principal = value;
        }

        public ClaimsPrincipal FindPrincipal()
        {
            return _principal ?? _httpContextAccessor.HttpContext?.User;
        }
    }
}