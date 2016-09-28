using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using OneTrueError.App;
using OneTrueError.App.Core.ApiKeys;

namespace OneTrueError.Web.Infrastructure.Auth
{
    public class ApiKeyAuthenticator : IAuthenticationFilter
    {
        public bool AllowMultiple { get { return true; } }

#pragma warning disable 1998
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
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
                var key = await repos.GetByKeyAsync(apiKeys.First());
                var content = await context.Request.Content.ReadAsByteArrayAsync();
                if (!key.ValidateSignature(tokens.First(), content))
                {
                    context.ErrorResult =
                        new AuthenticationFailureResult(
                            "Body could not be signed by the shared secret. Verify your client configuration.",
                            context.Request);
                    return;
                }

                var roles = key.AllowedApplications.Select(x => "Application_" + x).ToArray();
                var principal = new OneTruePrincipal(0, key.GeneratedKey, roles);
                principal.Identity.AuthenticationType = "ApiKey";
                context.Principal = principal;
                Thread.CurrentPrincipal = principal;
            }
        }

#pragma warning disable 1998
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
        }
    }
}