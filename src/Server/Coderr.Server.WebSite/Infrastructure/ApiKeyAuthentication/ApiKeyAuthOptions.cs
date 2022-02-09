using System;
using System.Data;
using Microsoft.AspNetCore.Authentication;

namespace Coderr.Server.WebSite.Infrastructure.ApiKeyAuthentication
{
    public class ApiKeyAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultSchemeName = "ApiKey";
        public string AuthenticationScheme = DefaultSchemeName;

        public Func<IDbConnection> OpenDb;
    }
}