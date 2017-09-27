using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

namespace codeRR.Server.Web.Infrastructure
{
    /// <summary>
    ///     Mediator to allow a custom implementation
    /// </summary>
    internal class SessionStoreMediator : IAuthenticationSessionStore
    {
        private readonly Dictionary<string, AuthenticationTicket> _tickets =
            new Dictionary<string, AuthenticationTicket>();

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var service = DependencyResolver.Current.GetService<IAuthenticationSessionStore>();
            if (service == null)
            {
                var guid = Guid.NewGuid().ToString("N");
                _tickets[guid] = ticket;
                return Task.FromResult(guid);
            }

            return service.StoreAsync(ticket);
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var service = DependencyResolver.Current.GetService<IAuthenticationSessionStore>();
            if (service == null)
            {
                _tickets[key] = ticket;
                return Task.FromResult<object>(null);
            }
            return service.RenewAsync(key, ticket);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var service = DependencyResolver.Current.GetService<IAuthenticationSessionStore>();
            if (service == null)
            {
                AuthenticationTicket ticket;
                return !_tickets.TryGetValue(key, out ticket)
                    ? Task.FromResult<AuthenticationTicket>(null)
                    : Task.FromResult(ticket);
            }

            return service.RetrieveAsync(key);
        }

        public Task RemoveAsync(string key)
        {
            var service = DependencyResolver.Current.GetService<IAuthenticationSessionStore>();
            if (service == null)
            {
                _tickets.Remove(key);
                return Task.FromResult<object>(null);
            }

            return service.RemoveAsync(key);
        }

        //TODO: Background thread that removes expired tokens.
    }
}