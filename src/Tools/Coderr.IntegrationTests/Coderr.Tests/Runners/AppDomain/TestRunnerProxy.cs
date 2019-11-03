using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coderr.Tests.Runners.AppDomain.DTO;

namespace Coderr.Tests.Runners.AppDomain
{
    internal class TestRunnerProxy : MarshalByRefObject
    {
        public void RunTest(RunTests instruction, TestResultReporter reporter)
        {
            var name = new AssemblyName(instruction.AssemblyName);
            var assembly = Assembly.Load(name);

            try
            {
                var discoverer = new TestDiscoverer();
                discoverer.Load(assembly, instruction.Source);

                WrapRun(instruction, reporter, discoverer, assembly);

            }
            catch (Exception ex)
            {
                reporter.AddResult(new TestResult(instruction.TestCases[0])
                {
                    ErrorStackTrace = ex.StackTrace,
                    ErrorMessage = ex.Message,
                    Result = TestOutcome.Failed,
                });
            }
        }

        private static void WrapRun(RunTests instruction, TestResultReporter reporter, TestDiscoverer discoverer,
            Assembly assembly)
        {
            var runner = new TestRunner(discoverer);
            runner.Load(new[] {assembly}).GetAwaiter().GetResult();

            var result = new List<TestResult>();
            foreach (var testCase in instruction.TestCases)
            {
                reporter.Started(testCase);
                var tc = discoverer.Find(testCase.TestClassName);
                var method = tc?.Methods.FirstOrDefault(x => x.Name == testCase.TestMethodName);
                if (tc == null || method == null)
                {
                    reporter.Ended(testCase, TestOutcome.NotFound);
                    reporter.AddResult(new TestResult(testCase) {Result = TestOutcome.NotFound});
                }

                var testResult = runner.Run(tc, method).GetAwaiter().GetResult();
                TestOutcome outcome;
                if (testResult.IsIgnored)
                    outcome = TestOutcome.Ignored;
                else if (testResult.IsSuccess)
                    outcome = TestOutcome.Success;
                else
                    outcome = TestOutcome.Failed;

                reporter.Ended(testCase, outcome);
                reporter.AddResult(new TestResult(testCase)
                {
                    Result = outcome,
                    ErrorStackTrace = testResult.Exception?.StackTrace,
                    ErrorMessage = testResult.Exception?.Message
                });
            }
        }
    }
}