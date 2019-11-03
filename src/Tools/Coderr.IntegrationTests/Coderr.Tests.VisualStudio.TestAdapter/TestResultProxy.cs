using System.Collections.Generic;
using System.Linq;
using Coderr.Tests.Runners.AppDomain;
using Coderr.Tests.Runners.AppDomain.DTO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using TestOutcome = Coderr.Tests.Runners.AppDomain.DTO.TestOutcome;
using TestResult = Coderr.Tests.Runners.AppDomain.DTO.TestResult;

namespace Coderr.Tests.VisualStudio.TestAdapter
{
    internal class TestResultProxy : ITestResultReceiver
    {
        private readonly IFrameworkHandle _frameworkHandle;
        private readonly IReadOnlyList<TestCase> _testCases;

        public TestResultProxy(IFrameworkHandle frameworkHandle, IReadOnlyList<TestCase> testCases)
        {
            _frameworkHandle = frameworkHandle;
            _testCases = testCases;
        }

        public void AddResult(TestResult testResult)
        {
            var testCase = FindTestCase(testResult.TestCase.FullyQualifiedName);
            _frameworkHandle.RecordResult(new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(testCase)
            {
                ErrorStackTrace = testResult.ErrorStackTrace,
                ErrorMessage = testResult.ErrorMessage,
                Outcome = ConvertOutcome(testResult.Result),
                Duration = testResult.Duration
            });
        }

        public void Ended(TestCaseToRun tc, TestOutcome outcome)
        {
            var testCase = FindTestCase(tc.FullyQualifiedName);
            _frameworkHandle.RecordEnd(testCase, ConvertOutcome(outcome));
        }

        public void Started(TestCaseToRun tc)
        {
            var testCase = FindTestCase(tc.FullyQualifiedName);
            _frameworkHandle.RecordStart(testCase);
        }

        private Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome ConvertOutcome(TestOutcome testOutcome)
        {
            switch (testOutcome)
            {
                case TestOutcome.Ignored:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Skipped;
                case TestOutcome.NotFound:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.NotFound;
                case TestOutcome.Success:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Passed;
                default:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Failed;
            }
        }

        private TestCase FindTestCase(string fullyQualifiedName)
        {
            var testCase = _testCases.FirstOrDefault(x => x.FullyQualifiedName == fullyQualifiedName);
            return testCase;
        }
    }
}