using Coderr.Tests.Runners.AppDomain.DTO;

namespace Coderr.Tests.Runners.AppDomain
{
    public interface ITestResultReceiver
    {
        void AddResult(TestResult testResult);

        void Ended(TestCaseToRun testCase, Runners.AppDomain.DTO.TestOutcome outcome);

        void Started(TestCaseToRun testCase);
    }
}