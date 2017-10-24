using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Container;
using log4net;

namespace codeRR.Server.Web.IoC
{
    public class GriffinContainerAdapter : IContainer
    {
        private readonly IParentContainer _container;
        private ILog _logger = LogManager.GetLogger(typeof(GriffinContainerAdapter));

        public GriffinContainerAdapter(IParentContainer container)
        {
            _container = container;
        }

        public IParentContainer InnerContainer { get { return _container; } }

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

        public IContainerScope CreateScope()
        {
            return new GriffinContainerScopeAdapter(_container.CreateChildContainer());
        }
    }
}