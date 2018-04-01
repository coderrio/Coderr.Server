using System;
using System.Collections.Generic;
using DotNetCqs.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Web.Boot.Adapters
{
    public class HandlerScopeWrapper : IHandlerScope
    {
        private readonly ServiceProvider _serviceProvider;

        public HandlerScopeWrapper(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        public IEnumerable<object> Create(Type messageHandlerServiceType)
        {
            return _serviceProvider.GetServices(messageHandlerServiceType);
        }

        public IEnumerable<T> ResolveDependency<T>()
        {
            return _serviceProvider.GetServices<T>();
        }
    }
}