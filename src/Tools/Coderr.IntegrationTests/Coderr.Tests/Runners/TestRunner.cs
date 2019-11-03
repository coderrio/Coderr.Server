using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.Tests.Attributes;
using Coderr.Tests.DependencyInjection;
using Coderr.Tests.Domain;
using Coderr.Tests.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Tests.Runners
{
    public class TestRunner
    {
        private readonly ITestDiscoverer _discoverer;
        private ServiceProvider _container;
        private IReadOnlyList<IAssemblyFixture> _fixtures;
        private Action<ServiceCollection> _registerCallback;

        public TestRunner(ITestDiscoverer discoverer)
        {
            _discoverer = discoverer;
        }

        public TestRunner()
        {
            _discoverer = null;
        }

        public async Task Load(IReadOnlyList<Assembly> assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var builder = new ServiceCollection();

            _fixtures = LoadFixtures(assemblies, builder);
            foreach (var assemblyFixture in _fixtures)
            {
                await assemblyFixture.Prepare();
                assemblyFixture.RegisterServices(builder);
            }

            var types = (from assembly in assemblies
                    from type in assembly.GetTypes()
                    let attribute = type.GetCustomAttribute<ContainerServiceAttribute>()
                    where attribute != null
                    select new {type, attribute}
                ).Distinct();

            foreach (var kvp in types)
            {
                var ifs = kvp.type.GetInterfaces();
                if (ifs.Length > 1 || ifs.Length == 0)
                    Register(builder, kvp.attribute.Lifetime, kvp.type, kvp.type);
                foreach (var @if in ifs)
                    Register(builder, kvp.attribute.Lifetime, @if, kvp.type);
            }

            // Now register all test classes 
            var testTypes = (from assembly in assemblies
                    from type in assembly.GetTypes()
                    from method in type.GetMethods()
                    where method.GetCustomAttribute<TestAttribute>() != null
                    select type
                ).Distinct();
            foreach (var type in testTypes) builder.AddScoped(type, type);


            _registerCallback?.Invoke(builder);

            _container = builder.BuildServiceProvider();
        }

        public void RegisterServices(Action<ServiceCollection> action)
        {
            _registerCallback = action;
        }

        public async Task Run<T>()
        {
            if (_discoverer == null)
                throw new InvalidOperationException("Requires that a discoverer was assigned in the constructor.");

            var testClass = await _discoverer.Find(typeof(T));
            if (testClass == null)
                throw new ArgumentOutOfRangeException(nameof(T), typeof(T), "Test class has not been registered");

            await RunTestClass(testClass);
        }

        public async Task<TestResult> Run(TestClass testClass, TestMethod method)
        {
            return await RunTestMethod(testClass, method);
        }

        public async Task<IReadOnlyList<TestResult>> RunAll()
        {
            if (_discoverer == null)
                throw new InvalidOperationException("Requires that a discoverer was assigned in the constructor.");

            var results = new List<TestResult>();
            var testClasses = await _discoverer.GetAll();
            foreach (var testClass in testClasses)
            {
                if (testClass.IsIgnored)
                    continue;

                var classResults = await RunTestClass(testClass);
                results.AddRange(classResults);
            }

            return results;
        }

        public async Task Shutdown()
        {
            foreach (var fixture in _fixtures) await fixture.Cleanup();

            _container.Dispose();
        }

        private IReadOnlyList<IAssemblyFixture> LoadFixtures(IReadOnlyList<Assembly> assemblies,
            ServiceCollection builder)
        {
            var result = new List<IAssemblyFixture>();
            foreach (var assembly in assemblies)
            {
                var fixtures = assembly.GetTypes().Where(x =>
                    typeof(IAssemblyFixture).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);
                foreach (var fixture in fixtures)
                {
                    var fix = (IAssemblyFixture) Activator.CreateInstance(fixture);
                    result.Add(fix);
                }
            }

            return result;
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

        private async Task<IReadOnlyList<TestResult>> RunTestClass(TestClass testClass)
        {
            var results = new List<TestResult>();
            foreach (var method in testClass.Methods)
            {
                if (method.IsIgnored)
                    continue;

                var result = await RunTestMethod(testClass, method);
                results.Add(result);
            }

            return results;
        }

        private async Task<TestResult> RunTestMethod(TestClass testClass, TestMethod method)
        {
            using (var scope = _container.CreateScope())
            {
                var tests = scope.ServiceProvider
                    .GetServices(testClass.Type)
                    .ToList();
                if (!tests.Any())
                {
                    throw new ArgumentOutOfRangeException(nameof(testClass), testClass,
                        $"{testClass.Type.FullName} has not been registered.");
                }

                var test = tests[0];

                var eventReceivers = scope.ServiceProvider.GetServices<IEventReceiver>();
                var aggregateReceiver = new AggregateEventReceiver(eventReceivers);
                return await testClass.Run(test, aggregateReceiver, method);
            }
        }
    }
}