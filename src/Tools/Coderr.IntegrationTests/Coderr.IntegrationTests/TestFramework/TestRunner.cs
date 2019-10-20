using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    internal class TestRunner
    {
        private ServiceProvider _container;
        private readonly List<TestClass> _testClasses = new List<TestClass>();
        private Action<ServiceCollection> _registerCallback;

        public void Load(IEnumerable<Assembly> assemblies)
        {
            var types = (from assembly in assemblies
                from type in assembly.GetTypes()
                from method in type.GetMethods()
                where method.GetCustomAttribute<TestAttribute>() != null
                      || type.GetCustomAttribute<ContainerServiceAttribute>() != null
                select type).Distinct();

            var builder = new ServiceCollection();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<ContainerServiceAttribute>();
                if (attr != null)
                {
                    var ifs = type.GetInterfaces();
                    if (ifs.Length > 1 || ifs.Length == 0)
                        Register(builder, attr.Lifetime, type, type);
                    foreach (var @if in ifs) Register(builder, attr.Lifetime, @if, type);
                }
                else
                {
                    _testClasses.Add(new TestClass(type));
                    builder.AddScoped(type);
                }
            }

            _registerCallback(builder);

            _container = builder.BuildServiceProvider();
        }

        public async Task Run<T>()
        {
            var testClass = _testClasses.FirstOrDefault(x => x.Type == typeof(T));
            if (testClass == null)
                throw new ArgumentOutOfRangeException(nameof(T), typeof(T), "Test class has not been registered");

            await RunTestClass(testClass);
        }

        public async Task RunAll()
        {
            foreach (var testClass in _testClasses) await RunTestClass(testClass);
        }

        private void Register(ServiceCollection builder, Lifetime lifetime, Type service, Type imp)
        {
            switch (lifetime)
            {
                case Lifetime.Scoped:
                    builder.AddScoped(service, imp);
                    break;
                case Lifetime.Transient:
                    builder.AddTransient(service, imp);
                    break;
                case Lifetime.SingleInstance:
                    builder.AddSingleton(service, imp);
                    break;
            }
        }

        private async Task RunTestClass(TestClass testClass)
        {
            using (var scope = _container.CreateScope())
            {
                var test = scope.ServiceProvider.GetService(testClass.Type);
                if (test == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(testClass), testClass, "Given test class has not been registered.");
                }

                var eventReceivers = scope.ServiceProvider.GetServices<IEventReceiver>();
                var aggregateReceiver = new AggregateEventReceiver(eventReceivers);
                await testClass.Run(test, aggregateReceiver);
            }
        }

        public void RegisterServices(Action<ServiceCollection> action)
        {
            _registerCallback = action;
        }
    }
}