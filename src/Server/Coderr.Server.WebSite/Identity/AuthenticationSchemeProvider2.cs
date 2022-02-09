using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Coderr.Server.WebSite.Identity
{
    public class AuthenticationSchemeProvider2 : IAuthenticationSchemeProvider
    {
        private List<AuthenticationScheme> _schemes = new List<AuthenticationScheme>();
        public void AddScheme(AuthenticationScheme scheme)
        {
            _schemes.Add(scheme);
        }

        public Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
        {
            return Task.FromResult<IEnumerable<AuthenticationScheme>>(_schemes);
        }

        public Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync()
        {
            return Task.FromResult(_schemes.FirstOrDefault());
        }

        public Task<AuthenticationScheme> GetDefaultChallengeSchemeAsync()
        {
            return Task.FromResult(_schemes.FirstOrDefault());
        }

        public Task<AuthenticationScheme> GetDefaultForbidSchemeAsync()
        {
            return Task.FromResult(_schemes.FirstOrDefault());
        }

        public Task<AuthenticationScheme> GetDefaultSignInSchemeAsync()
        {
            return Task.FromResult(_schemes.FirstOrDefault());
        }

        public Task<AuthenticationScheme> GetDefaultSignOutSchemeAsync()
        {
            return Task.FromResult(_schemes.FirstOrDefault());
        }

        public Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
        {
            return Task.FromResult<IEnumerable<AuthenticationScheme>>(_schemes);
        }

        public Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
            return Task.FromResult(_schemes.FirstOrDefault(x=>x.Name==name));
        }

        public void RemoveScheme(string name)
        {
            _schemes.RemoveAll(x => x.Name == name);
        }
    }
}