using System.Reflection;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Web.Boot.Cqs;

namespace Coderr.Server.Web.Boot.Adapters
{
    public class CqsObjectMapperConfigurationContext : ConfigurationContext
    {
        private readonly CqsObjectMapper _cqsObjectMapper;

        public CqsObjectMapperConfigurationContext(CqsObjectMapper cqsObjectMapper)
        {
            _cqsObjectMapper = cqsObjectMapper;
        }

        public override void RegisterMessageTypes(Assembly assembly)
        {
            _cqsObjectMapper.ScanAssembly(assembly);
        }
    }
}