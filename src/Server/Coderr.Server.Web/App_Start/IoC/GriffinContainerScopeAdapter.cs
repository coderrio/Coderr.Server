using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Griffin.Container;
using log4net;

namespace codeRR.Server.Web.IoC
{
    public sealed class GriffinContainerScopeAdapter : IContainerScope
    {
        private static int _currentScopes;
        private readonly IChildContainer _container;
        private bool _disposed;
        private ILog _logger = LogManager.GetLogger(typeof(GriffinContainerScopeAdapter));

        public GriffinContainerScopeAdapter(IChildContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            Interlocked.Increment(ref _currentScopes);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            Interlocked.Decrement(ref _currentScopes);
            _container.Dispose();
        }

        public TService Resolve<TService>()
        {
            return (TService) _container.Resolve(typeof(TService));
        }

        public object Resolve(Type service)
        {
            return _container.Resolve(service);
        }

        public IEnumerable<TService> ResolveAll<TService>()
        {
            return _container.ResolveAll(typeof(TService)).Cast<TService>();
        }

        public IEnumerable<object> ResolveAll(Type service)
        {
            return _container.ResolveAll(service);
        }
    }
}