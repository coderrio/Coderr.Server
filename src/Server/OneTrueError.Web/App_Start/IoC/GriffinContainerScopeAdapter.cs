using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Griffin.Container;
using log4net;

namespace OneTrueError.Web.IoC
{
    public sealed class GriffinContainerScopeAdapter : IContainerScope
    {
        private static int CurrentScopes;
        private readonly IChildContainer _container;
        private bool _disposed;
        private ILog _logger = LogManager.GetLogger(typeof(GriffinContainerScopeAdapter));

        public GriffinContainerScopeAdapter(IChildContainer container)
        {
            _container = container;
            Interlocked.Increment(ref CurrentScopes);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            Interlocked.Decrement(ref CurrentScopes);
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