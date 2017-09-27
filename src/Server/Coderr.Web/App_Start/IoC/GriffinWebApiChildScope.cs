using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Griffin.Container;

namespace codeRR.Server.Web.IoC
{
    public sealed class GriffinWebApiChildScope : IDependencyScope
    {
        private readonly IChildContainer _childContainer;

        public GriffinWebApiChildScope(IChildContainer childContainer)
        {
            _childContainer = childContainer;
        }

        public void Dispose()
        {
            _childContainer.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return GetServices(serviceType).FirstOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _childContainer.ResolveAll(serviceType);
        }
    }
}