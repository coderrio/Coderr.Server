using System;
using System.IO;
using System.Reflection;
using Coderr.Tests.Runners.AppDomain;
using Coderr.Tests.Runners.AppDomain.DTO;

namespace Coderr.Tests.Runners
{
    public class AppDomainRunner : IDisposable
    {
        private readonly string _name;
        private System.AppDomain _domain;
        private AssemblyLoaderProxy _assemblyLoaderProxy;
        private TestRunnerProxy _runnerProxy;

        public AppDomainRunner(string name)
        {
            _name = name;
        }

        public void Create()
        {
            string pathToDll = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var domainSetup = new AppDomainSetup
            {
                PrivateBinPath = pathToDll,
                ConfigurationFile = "",
                ApplicationBase = System.AppDomain.CurrentDomain.BaseDirectory
            };

            var evidence = System.AppDomain.CurrentDomain.Evidence;
            _domain = System.AppDomain.CreateDomain(_name, evidence, domainSetup);

            _assemblyLoaderProxy = LoadProxy<AssemblyLoaderProxy>();
            _assemblyLoaderProxy.AddDirectory(pathToDll);
            _runnerProxy = LoadProxy<TestRunnerProxy>();
        }

        public void RunTests(RunTests command, ITestResultReceiver reporter)
        {
            _assemblyLoaderProxy.AddDirectory(command.AssemblyPath);
            var ourProxy = new TestResultReporter(reporter);
            _runnerProxy.RunTest(command, ourProxy);
        }

        private T LoadProxy<T>() where T : MarshalByRefObject
        {
            var type = typeof(T);
            return (T)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
        }

        public void Dispose()
        {
            System.AppDomain.Unload(_domain);
            _domain = null;
        }

        public void AddDirectory(string directoryName)
        {
            _assemblyLoaderProxy.AddDirectory(directoryName);
        }
    }
}
