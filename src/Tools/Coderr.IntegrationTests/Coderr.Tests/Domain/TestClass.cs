using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.Tests.Attributes;
using Coderr.Tests.Events;

namespace Coderr.Tests.Domain
{
    public class TestClass
    {
        public TestClass(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));

            Methods = (from method in type.GetMethods()
                    where method.GetCustomAttribute<TestAttribute>() != null
                    select new TestMethod(this, method)
                ).ToList();

            InitMethod = type.GetMethods()
                .FirstOrDefault(x => x.GetCustomAttribute<TestInitAttribute>() != null);
        }

        public TestClass(Type type, string source)
            : this(type)
        {
            Source = source;
        }

        public MethodInfo InitMethod { get; set; }

        public bool IsIgnored { get; set; }

        public IReadOnlyList<TestMethod> Methods { get; set; }

        /// <summary>
        ///     When loading assemblies from a test runner, save from where this test were loaded
        /// </summary>
        public string Source { get; }

        public Type Type { get; set; }

        public bool GotExclusiveFlag()
        {
            return Type.GetCustomAttribute<RunOnlyThisOneAttribute>() != null
                   || Methods.Any(x => x.GetCustomAttribute<RunOnlyThisOneAttribute>() != null);
        }

        public void MarkNonExclusive()
        {
            // Whole class should be run
            if (Type.GetCustomAttribute<RunOnlyThisOneAttribute>() != null)
                return;

            // If no tests got it, we should ignore everything
            if (Methods.All(x => x.GetCustomAttribute<RunOnlyThisOneAttribute>() == null))
            {
                foreach (var method in Methods)
                    method.IsIgnored = true;

                IsIgnored = true;
                return;
            }

            // Now ignore all that have not the attribute.
            foreach (var testMethod in Methods)
                testMethod.IsIgnored = testMethod.GetCustomAttribute<RunOnlyThisOneAttribute>() == null;
        }

        public async Task<TestResult> Run(object testClassInstance, AggregateEventReceiver aggregateReceiver,
            TestMethod specificMethod)
        {
            await ExecuteTestInit(testClassInstance);
            return await specificMethod.Invoke(testClassInstance, aggregateReceiver);
        }

        public async Task<List<TestResult>> Run(object testClassInstance, AggregateEventReceiver aggregateReceiver)
        {
            aggregateReceiver.Handle(new TestClassStarted(testClassInstance));
            var results = new List<TestResult>();
            foreach (var method in Methods)
            {
                await ExecuteTestInit(testClassInstance);
                var result = await method.Invoke(testClassInstance, aggregateReceiver);
                results.Add(result);
            }

            var totalTime = results.Sum(x => x.Elapsed.TotalMilliseconds);
            aggregateReceiver.Handle(new TestClassFinished(testClassInstance, TimeSpan.FromMilliseconds(totalTime)));
            return results;
        }


        public override string ToString()
        {
            return $"{Type.FullName}";
        }

        private async Task ExecuteTestInit(object testClassInstance)
        {
            var result = InitMethod?.Invoke(testClassInstance, null);
            if (result is Task t) await t;
        }
    }
}