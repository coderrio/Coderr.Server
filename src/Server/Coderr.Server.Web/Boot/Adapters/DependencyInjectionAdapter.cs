using System;
using System.Collections.Generic;
using Griffin.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Web.Boot.Adapters
{
    public class DependencyInjectionAdapter : IContainer
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionAdapter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TService Resolve<TService>()
        {
            return _serviceProvider.GetService<TService>();
        }

        public object Resolve(Type service)
        {
            return _serviceProvider.GetService(service);
        }

        public IEnumerable<TService> ResolveAll<TService>()
        {
            return _serviceProvider.GetServices<TService>();
        }

        public IEnumerable<object> ResolveAll(Type service)
        {
            return _serviceProvider.GetServices(service);
        }

        public IContainerScope CreateScope()
        {
            var scope = _serviceProvider.CreateScope();
            return new ServiceScopeWrapper(scope);
        }
    }
}