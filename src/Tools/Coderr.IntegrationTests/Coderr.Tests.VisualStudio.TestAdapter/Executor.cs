using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Coderr.Tests.Domain;
using Coderr.Tests.Runners;
using Coderr.Tests.Runners.AppDomain.DTO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using TestOutcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace Coderr.Tests.VisualStudio.TestAdapter
{
    [ExtensionUri(Constants.ExecutorUri)]
    public class Executor : ITestExecutor
    {
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            try
            {
                var perSource = tests.GroupBy(x => x.Source);
                foreach (var source in perSource)
                {
                    RunSource(frameworkHandle, source);
                }
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, ex.ToString());
            }
        }

        private static void RunSource(IFrameworkHandle frameworkHandle, IGrouping<string, TestCase> source)
        {
            var tests = source.ToList();
            if (true)
            {
                RunInAppDomain(frameworkHandle, source, tests);
            }
            else
            {
                RunLocally(frameworkHandle, source);
            }

        }

        private static void RunLocally(IFrameworkHandle frameworkHandle, IGrouping<string, TestCase> source)
        {
            var discoverer = new TestDiscoverer();
            var tests2 = discoverer.LoadFromSources(new[] { source.Key },
                x => frameworkHandle.SendMessage(TestMessageLevel.Informational, x));
            if (!tests2.Any())
                return;

            var runner = new TestRunner(discoverer);
            runner.Load(new[] {tests2[0].Type.Assembly}).GetAwaiter().GetResult();

            foreach (var testCase in source)
            {
                var testClassName = (string) testCase.GetPropertyValue(Constants.TestClassProperty);
                var testName = (string) testCase.GetPropertyValue(Constants.TestMethodProperty);

                var method = discoverer.Get(testClassName, testName);
                if (method == null)
                {
                    frameworkHandle.RecordResult(new TestResult(testCase)
                    {
                        Outcome = TestOutcome.NotFound
                    });
                    continue;
                }

                RunTestMethod(frameworkHandle, testCase, runner, method);
            }

            runner.Shutdown().GetAwaiter().GetResult();
        }

        private static void RunInAppDomain(IFrameworkHandle frameworkHandle, IGrouping<string, TestCase> source, List<TestCase> tests)
        {
            var dir = Path.GetDirectoryName(source.Key);
            var cmd = new RunTests
            {
                AssemblyName = Path.GetFileName(source.Key), AssemblyPath = dir, Source = source.Key,
            };
            var dto = new List<TestCaseToRun>();
            foreach (TestCase testCase in tests)
            {
                var parts = testCase.FullyQualifiedName.Split('.');
                dto.Add(new TestCaseToRun(testCase.FullyQualifiedName)
                {
                    TestClassName = parts[parts.Length - 2],
                    TestMethodName = parts[parts.Length - 1]
                });
            }

            using (var appDomainRunner = new AppDomainRunner(source.Key))
            {
                appDomainRunner.AddDirectory(dir);
                appDomainRunner.Create();


                var resultProxy = new TestResultProxy(frameworkHandle, tests);
                appDomainRunner.RunTests(cmd, resultProxy);
            }
        }

        private static void RunTestMethod(IFrameworkHandle frameworkHandle, TestCase testCase, TestRunner runner,
            TestMethod method)
        {
            frameworkHandle.RecordStart(testCase);
            try
            {
                var result = runner.Run(method.Owner, method).GetAwaiter().GetResult();
                if (result == null)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Warning, "Got no result");
                    return;
                }

                var msResult = new TestResult(testCase)
                {
                    StartTime = result.StartedAtUtc,
                    EndTime = result.EndedAtUtc,
                    DisplayName = method.Name.Replace("_", " "),
                    Outcome = TestOutcome.Passed,
                    Duration = result.Elapsed,
                    ErrorMessage = result.Exception?.Message,
                    ErrorStackTrace = result.Exception?.StackTrace
                };

                if (result.IsIgnored)
                {
                    msResult.Outcome = TestOutcome.Skipped;
                }
                else if (result.IsSuccess)
                {
                    msResult.Outcome = TestOutcome.Passed;
                }
                else
                {
                    msResult.Outcome = TestOutcome.Failed;
                }

                frameworkHandle.RecordEnd(testCase, msResult.Outcome);
                frameworkHandle.RecordResult(msResult);
            }
            catch (Exception ex)
            {
                frameworkHandle.RecordEnd(testCase, TestOutcome.Failed);
                frameworkHandle.RecordResult(new TestResult(testCase)
                {
                    DisplayName = method.Name.Replace("_", " "),
                    Outcome = TestOutcome.Failed,
                    ErrorMessage = ex.Message,
                    ErrorStackTrace = ex.StackTrace
                });
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running sources directly");
        }

        public void Cancel()
        {

        }
    }
}
