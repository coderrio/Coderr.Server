using System;
using System.Collections.Generic;
using Griffin.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.WebSite.Infrastructure.Adapters
{
    public class ServiceScopeWrapper : IContainerScope
    {
        private readonly IServiceScope _scope;

        public ServiceScopeWrapper(IServiceScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public TService Resolve<TService>()
        {
            return _scope.ServiceProvider.GetService<TService>();
        }

        public object Resolve(Type service)
        {
            return _scope.ServiceProvider.GetService(service);
        }

        public IEnumerable<TService> ResolveAll<TService>()
        {
            return _scope.ServiceProvider.GetServices<TService>();
        }

        public IEnumerable<object> ResolveAll(Type service)
        {
            return _scope.ServiceProvider.GetServices(service);
        }
    }
}