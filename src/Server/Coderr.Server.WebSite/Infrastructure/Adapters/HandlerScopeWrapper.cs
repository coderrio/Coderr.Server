using System;
using System.Collections.Generic;
using DotNetCqs.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.WebSite.Infrastructure.Adapters
{
    public class HandlerScopeWrapper : IHandlerScope
    {
        private readonly IServiceProvider _serviceProvider;

        public HandlerScopeWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            var disp = _serviceProvider as IDisposable;
            disp?.Dispose();
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