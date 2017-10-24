using System;
using codeRR.Server.Web.IoC;
using DotNetCqs.DependencyInjection;
using Griffin.Container;

namespace codeRR.Server.Web.Cqs
{
    public class GriffinHandlerScopeFactory : IHandlerScopeFactory
    {
        private readonly IContainer _container;

        public GriffinHandlerScopeFactory(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IHandlerScope CreateScope()
        {
            var scope = _container.CreateScope();
            return (GriffinContainerScopeAdapter)scope;
        }
    }
}