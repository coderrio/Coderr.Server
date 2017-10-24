using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using codeRR.Server.App.Core.ApiKeys;
using codeRR.Server.Infrastructure.Security;
using Griffin.Data;

namespace codeRR.Server.Web.Infrastructure.Auth
{
    public class ApiKeyAuthenticator : IAuthenticationFilter
    {
        public bool AllowMultiple
        {
            get { return true; }
        }

#pragma warning disable 1998
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            var p = context.Principal as ClaimsPrincipal;
            if (p != null && p.Identity.IsAuthenticated)
                return;

            IEnumerable<string> apiKeys;
            IEnumerable<string> tokens;
            if (!context.Request.Headers.TryGetValues("X-Api-Key", out apiKeys) ||
                !context.Request.Headers.TryGetValues("X-Api-Signature", out tokens))
            {
                return;
            }

            using (var scope = GlobalConfiguration.Configuration.DependencyResolver.BeginScope())
            {
                var repos = (IApiKeyRepository) scope.GetService(typeof(IApiKeyRepository));
                ApiKey key;
                try
                {
                    key = await repos.GetByKeyAsync(apiKeys.First());
                }
                catch (EntityNotFoundException)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Invalid/unknown API key", context.Request);
                    return;
                }

                var content = await context.Request.Content.ReadAsByteArrayAsync();
                if (!key.ValidateSignature(tokens.First(), content))
                {
                    context.ErrorResult =
                        new AuthenticationFailureResult(
                            "Body could not be signed by the shared secret. Verify your client configuration.",
                            context.Request);
                    return;
                }

                var claims = key.Claims;
                var factoryContext = new PrincipalFactoryContext(0, key.GeneratedKey, new[] {CoderrClaims.RoleSystem})
                {
                    AuthenticationType = "ApiKey",
                    Claims = claims
                };
                var principal = await PrincipalFactory.CreateAsync(factoryContext);
                context.Principal = principal;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }
    }
}