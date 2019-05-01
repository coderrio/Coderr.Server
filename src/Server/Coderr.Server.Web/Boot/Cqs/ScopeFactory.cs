using System;
using DotNetCqs.DependencyInjection;
using DotNetCqs.DependencyInjection.Microsoft;

namespace Coderr.Server.Web.Boot.Cqs
{
    internal class ScopeFactory : IHandlerScopeFactory
    {
        private readonly Func<IServiceProvider> _serviceProviderAccesor;
        private MicrosoftHandlerScopeFactory _scopeFactory;

        public ScopeFactory(Func<IServiceProvider> serviceProviderAccesor)
        {
            _serviceProviderAccesor = serviceProviderAccesor;
        }

        public IHandlerScope CreateScope()
        {
            if (_scopeFactory == null)
            {
                var provider = _serviceProviderAccesor();
                if (provider == null)
                    throw new InvalidOperationException("container have not been setup properly yet.");
                _scopeFactory = new MicrosoftHandlerScopeFactory(provider);
            }

            return _scopeFactory.CreateScope();
        }
    }
}
