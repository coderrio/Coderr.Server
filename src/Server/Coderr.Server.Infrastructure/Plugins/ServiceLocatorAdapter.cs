using Griffin.Container;

namespace codeRR.Server.Infrastructure.Plugins
{
    public class ServiceLocatorAdapter : IScopedServiceLocator
    {
        private readonly IServiceLocator _locator;

        public ServiceLocatorAdapter(IServiceLocator locator)
        {
            _locator = locator;
        }

        public T GetService<T>() where T : class
        {
            return _locator.Resolve<T>();
        }
    }
}