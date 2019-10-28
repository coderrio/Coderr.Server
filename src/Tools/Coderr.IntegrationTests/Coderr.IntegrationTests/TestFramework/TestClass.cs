using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.TestFramework.Events;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    public class TestClass
    {
        public TestClass(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Methods = type.GetMethods()
                .Where(x => x.GetCustomAttribute<TestAttribute>() != null)
                .Select(x => new TestMethod(x))
                .ToList();
            InitMethod = type.GetMethods()
                .FirstOrDefault(x => x.GetCustomAttribute<TestInitAttribute>() != null);
        }

        public MethodInfo InitMethod { get; set; }

        public IReadOnlyList<TestMethod> Methods { get; set; }

        public Type Type { get; set; }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return Type.GetCustomAttribute<T>();
        }

        public async Task Run(object testClassInstance, AggregateEventReceiver aggregateReceiver,
            TestMethod specificMethod = null)
        {
            aggregateReceiver.Handle(new TestClassStarted(testClassInstance));


            var sw = Stopwatch.StartNew();
            if (specificMethod != null)
            {
                await ExecuteTestInit(testClassInstance);
                await specificMethod.Invoke(testClassInstance, aggregateReceiver);
            }
            else
            {
                foreach (var method in Methods)
                {
                    await ExecuteTestInit(testClassInstance);
                    await method.Invoke(testClassInstance, aggregateReceiver);
                }
            }

            sw.Stop();
            aggregateReceiver.Handle(new TestClassFinished(testClassInstance, sw.Elapsed));
        }

        private async Task ExecuteTestInit(object testClassInstance)
        {
            var result = InitMethod?.Invoke(testClassInstance, null);
            if (result is Task t) await t;
        }
    }
}