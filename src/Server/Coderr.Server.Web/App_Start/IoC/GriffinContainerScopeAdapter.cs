using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetCqs.DependencyInjection;
using Griffin.Container;
using log4net;

namespace codeRR.Server.Web.IoC
{
    public sealed class GriffinContainerScopeAdapter : IContainerScope, IHandlerScope
    {
        private static int _currentScopes;
        private readonly IChildContainer _scope;
        private bool _disposed;
        private ILog _logger = LogManager.GetLogger(typeof(GriffinContainerScopeAdapter));

        public GriffinContainerScopeAdapter(IChildContainer container)
        {
            _scope = container ?? throw new ArgumentNullException(nameof(container));
            Interlocked.Increment(ref _currentScopes);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            Interlocked.Decrement(ref _currentScopes);
            _scope.Dispose();
        }

        public TService Resolve<TService>()
        {
            return (TService) _scope.Resolve(typeof(TService));
        }

        public object Resolve(Type service)
        {
            return _scope.Resolve(service);
        }

        public IEnumerable<TService> ResolveAll<TService>()
        {
            return _scope.ResolveAll(typeof(TService)).Cast<TService>();
        }

        public IEnumerable<object> ResolveAll(Type service)
        {
            return _scope.ResolveAll(service);
        }

        public IEnumerable<object> Create(Type messageHandlerServiceType)
        {
            return _scope.ResolveAll(messageHandlerServiceType);
        }

        public IEnumerable<T> ResolveDependency<T>()
        {
            return _scope.ResolveAll(typeof(T)).Cast<T>();
        }
    }
}