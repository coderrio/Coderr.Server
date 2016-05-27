using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Container;

namespace OneTrueError.Web.IoC
{
    public sealed class GriffinContainerScopeAdapter : IContainerScope
    {
        private readonly IChildContainer _container;

        public GriffinContainerScopeAdapter(IChildContainer container)
        {
            _container = container;
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public TService Resolve<TService>()
        {
            return (TService)_container.Resolve(typeof(TService));
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