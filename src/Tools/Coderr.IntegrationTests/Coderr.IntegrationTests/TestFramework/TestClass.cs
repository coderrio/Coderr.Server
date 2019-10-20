using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        }

        public IReadOnlyList<TestMethod> Methods { get; set; }

        public Type Type { get; set; }

        public async Task Run(object testClassInstance, AggregateEventReceiver aggregateReceiver)
        {
            aggregateReceiver.Handle(new TestClassStarted(testClassInstance));
            var sw = Stopwatch.StartNew();
            foreach (var method in Methods)
            {
                await method.Invoke(testClassInstance, aggregateReceiver);
            }

            sw.Stop();
            aggregateReceiver.Handle(new TestClassFinished(testClassInstance, sw.Elapsed));
        }
    }
}