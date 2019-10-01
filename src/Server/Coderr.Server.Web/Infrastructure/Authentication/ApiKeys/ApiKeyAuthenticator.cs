using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.App.Core.ApiKeys;
using Coderr.Server.SqlServer.Core.ApiKeys;
using Griffin.Data;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Coderr.Server.Web.Infrastructure.Authentication.ApiKeys
{
    public class ApiKeyAuthenticator : AuthenticationHandler<ApiKeyAuthOptions>
    {
        private readonly IOptionsMonitor<ApiKeyAuthOptions> _options;
        private ILog _logger = LogManager.GetLogger(typeof(ApiKeyAuthenticator));

        public bool AllowMultiple => true;

        public ApiKeyAuthenticator(IOptionsMonitor<ApiKeyAuthOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _options = options;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claimsPrincipal = Context.User;
            if (claimsPrincipal != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                _logger.Debug("Already authenticated.");
                return AuthenticateResult.NoResult();
            }

            var ticket = await CreateTicket(Request, _options.CurrentValue);
            return AuthenticateResult.Success(ticket);
        }

        public static async Task<AuthenticationTicket> CreateTicket(HttpRequest request, ApiKeyAuthOptions options)
        {
            var apiKeyHeader = request.Headers["X-Api-Key"];
            var signatureHeader = request.Headers["X-Api-Signature"];

            if (apiKeyHeader.Count == 0 ||
                signatureHeader.Count == 0)
                return null;
            var apiKey = apiKeyHeader[0];

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, apiKey),
                new Claim(ClaimTypes.Name, "ApiKey"),
                new Claim(ClaimTypes.Role, CoderrRoles.System)
            };
            var identity = new ClaimsIdentity(claims, options.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);


            using (var con = options.OpenDb())
            {
                using (var uow = new AdoNetUnitOfWork(con, false))
                {
                    var repos = new ApiKeyRepository(uow);
                    ApiKey key;
                    try
                    {
                        key = await repos.GetByKeyAsync(apiKey);
                    }
                    catch (EntityNotFoundException)
                    {
                        return null;
                    }

                    request.EnableRewind();

                    var buf = new byte[request.ContentLength ?? 0];
                    var bytesRead = await request.Body.ReadAsync(buf, 0, buf.Length);
                    request.Body.Position = 0;
                    if (!key.ValidateSignature(signatureHeader[0], buf))
                    {
                        return null;
                    }

                    if (key.Claims != null)
                    {
                        identity.AddClaims(key.Claims);
                    }

                    uow.SaveChanges();
                }
            }

            return new AuthenticationTicket(claimsPrincipal, options.AuthenticationScheme);
        }
    }

}