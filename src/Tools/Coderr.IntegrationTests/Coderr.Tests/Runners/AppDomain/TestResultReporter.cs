using System;
using Coderr.Tests.Runners.AppDomain.DTO;

namespace Coderr.Tests.Runners.AppDomain
{
    public class TestResultReporter : MarshalByRefObject
    {
        private ITestResultReceiver _receiver;

        public TestResultReporter(ITestResultReceiver receiver)
        {
            _receiver = receiver;
        }

        public void AddResult(TestResult testResult)
        {
           _receiver.AddResult(testResult);
        }

        public void Started(TestCaseToRun testCase)
        {
            _receiver.Started(testCase);
        }

        public void Ended(TestCaseToRun testCase, TestOutcome outcome)
        {
            _receiver.Ended(testCase, outcome);
        }
    }
}
