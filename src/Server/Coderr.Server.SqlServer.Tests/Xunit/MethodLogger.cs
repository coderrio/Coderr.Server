using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace codeRR.Server.SqlServer.Tests.Xunit
{
    public class MethodLogger: XunitTestClassRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;
        private ILog _logger = LogManager.GetLogger(typeof(MethodLogger));
        internal static ITestOutputHelper OutputHelper;
        public MethodLogger(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings) : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override async Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases,
            object[] constructorArguments)
        {
            try
            {
                OutputHelper?.WriteLine($"Running {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}");
                _logger.Info($"Running {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}");
                _diagnosticMessageSink.OnMessage(
                    new DiagnosticMessage($"Running {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}"));

                var t = base.RunTestMethodAsync(testMethod, method, testCases, constructorArguments);
                var delay = Task.Delay(5000);
                await Task.WhenAny(t, delay);
                if (!t.IsCompleted)
                {
                    throw new TimeoutException($"Timeout: {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}");
                }

                //var result= await base.RunTestMethodAsync(testMethod, method, testCases, constructorArguments);
                OutputHelper?.WriteLine($"..completed {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}");
                _logger.Info(".. completed " + testMethod.TestClass.Class.Name + "." + testMethod.Method.Name);
                _diagnosticMessageSink.OnMessage(
                    new DiagnosticMessage($".. completed {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}"));
                return t.Result;
            }
            catch (Exception e)
            {
                OutputHelper?.WriteLine($"..failed {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}");
                _logger.Info(".. failed " + testMethod.TestClass.Class.Name + "." + testMethod.Method.Name, e);
                _diagnosticMessageSink.OnMessage(
                    new DiagnosticMessage($".. failed {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} failed: " + e));
                throw;
            }
            
        }
    }
}