using System.Collections.Generic;
using System.Reflection;
using Griffin.Cqs.InversionOfControl;

namespace codeRR.Server.Web.Cqs
{
    /// <summary>
    ///     This is a workaround, I KNOW! Fix the problem!
    /// </summary>
    public class MyEventHandlerRegistry : EventHandlerRegistry
    {
        private readonly List<Assembly> _scannedAssemblies = new List<Assembly>();

        public new void ScanAssembly(Assembly assembly)
        {
            if (_scannedAssemblies.Contains(assembly))
                return;
            _scannedAssemblies.Add(assembly);
            base.ScanAssembly(assembly);
        }
    }
}