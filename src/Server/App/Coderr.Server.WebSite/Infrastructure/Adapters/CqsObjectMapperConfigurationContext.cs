using System;
using System.Reflection;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.WebSite.Infrastructure.Cqs;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.WebSite.Infrastructure.Adapters
{
    public class CqsObjectMapperConfigurationContext : ConfigurationContext
    {
        private readonly CqsObjectMapper _cqsObjectMapper;

        public CqsObjectMapperConfigurationContext(CqsObjectMapper cqsObjectMapper, IServiceCollection services, Func<IServiceProvider> serviceProvider) : base(services, serviceProvider)
        {
            _cqsObjectMapper = cqsObjectMapper;
        }

        public override void RegisterMessageTypes(Assembly assembly)
        {
            _cqsObjectMapper.ScanAssembly(assembly);
        }

        public override ConfigurationContext Clone(IServiceCollection serviceCollection)
        {
            return new CqsObjectMapperConfigurationContext(_cqsObjectMapper, serviceCollection, ServiceProvider)
            {
                Configuration = Configuration, ConnectionFactory = ConnectionFactory
            };
        }
    }
}