using System;

namespace Coderr.Server.Abstractions.WorkItems
{
    /// <summary>
    /// Used to register services for work items (each integration have its own service).
    /// </summary>
    public interface IWorkItemServiceRegistrar
    {
        void Register<T>(string integrationName) where T : IWorkItemService;

        Type GetServiceType(string integrationName);
    }
}