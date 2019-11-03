using System;
using Coderr.Tests.Runners.AppDomain;
using Coderr.Tests.Runners.AppDomain.DTO;

namespace ConsoleApp1
{
    internal class ConsoleReceiver : ITestResultReceiver
    {
        public void AddResult(TestResult testResult)
        {
            Console.WriteLine(testResult.TestCase.TestMethodName + ": " + testResult.Result + " " + testResult.ErrorMessage);
        }

        public void Ended(TestCaseToRun testCase, TestOutcome outcome)
        {
            Console.WriteLine("Ended");
        }

        public void Started(TestCaseToRun testCase)
        {
            Console.WriteLine("Started");
        }
    }
}