using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Griffin.Container;
using codeRR.GlobalCore.App.Setup;

namespace codeRR.Web
{
    public class GriffinWebApiDependencyResolver2 : IDependencyResolver
    {
        private readonly Container _ioc;

        public GriffinWebApiDependencyResolver2(Container ioc)
        {
            _ioc = ioc;
        }

        public void Dispose()
        {

        }

        public object GetService(Type serviceType)
        {
            return GetServices(serviceType).FirstOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _ioc.ResolveAll(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return new GriffinWebApiChildScope(_ioc.CreateChildContainer());
        }
    }
}