using System;

namespace Coderr.Tests.Runners.AppDomain.DTO
{
    [Serializable]
    public class TestResult
    {
        public TestResult(TestCaseToRun tc)
        {
            TestCase = tc;
        }

        public TimeSpan Duration { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStackTrace { get; set; }

        public TestOutcome Result { get; set; }
        public TestCaseToRun TestCase { get; private set; }
    }
}