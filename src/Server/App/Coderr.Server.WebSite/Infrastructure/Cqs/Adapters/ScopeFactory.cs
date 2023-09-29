using System;
using DotNetCqs.DependencyInjection;
using DotNetCqs.DependencyInjection.Microsoft;

namespace Coderr.Server.WebSite.Infrastructure.Cqs.Adapters
{
    internal class ScopeFactory : IHandlerScopeFactory
    {
        private readonly Func<IServiceProvider> _serviceProviderAccessor;
        private MicrosoftHandlerScopeFactory _scopeFactory;

        public ScopeFactory(Func<IServiceProvider> serviceProviderAccessor)
        {
            _serviceProviderAccessor = serviceProviderAccessor;
        }

        public IHandlerScope CreateScope()
        {
            if (_scopeFactory == null)
            {
                var provider = _serviceProviderAccessor();
                if (provider == null)
                    throw new InvalidOperationException("container have not been setup properly yet.");
                _scopeFactory = new MicrosoftHandlerScopeFactory(provider);
            }

            return _scopeFactory.CreateScope();
        }
    }
}
