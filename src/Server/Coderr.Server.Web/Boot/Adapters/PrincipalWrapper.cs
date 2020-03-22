using System;
using System.Security;
using System.Security.Claims;
using Coderr.Server.Abstractions.Security;
using Microsoft.AspNetCore.Http;

namespace Coderr.Server.Web.Boot.Adapters
{
    internal class PrincipalWrapper : IPrincipalAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal _principal;

        public PrincipalWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal Principal
        {
            get
            {
                if (_principal != null)
                    return _principal;

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