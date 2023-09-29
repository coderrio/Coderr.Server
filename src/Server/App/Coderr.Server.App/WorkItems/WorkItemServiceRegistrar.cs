using System;
using System.Collections.Generic;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.WorkItems;

namespace Coderr.Server.App.WorkItems
{
    [ContainerService(IsSingleInstance = true)]
    public class WorkItemServiceRegistrar : IWorkItemServiceRegistrar
    {
        private readonly Dictionary<string, Type> _services = new Dictionary<string, Type>();

        public void Register<T>(string integrationName) where T : IWorkItemService
        {
            _services[integrationName] = typeof(T);
        }

        public Type GetServiceType(string integrationName)
        {
            if (!_services.TryGetValue(integrationName, out var type))
            {
                throw new InvalidOperationException($"Integration {integrationName} do not exist.");
            }

            return type;
        }
    }
}