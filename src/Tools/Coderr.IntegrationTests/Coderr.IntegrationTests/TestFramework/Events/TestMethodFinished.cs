using System;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    public class TestMethodFinished
    {
        public TestMethodFinished(object instance, TestMethod testMethod, TimeSpan executionTime)
        {
            Instance = instance;
            TestMethod = testMethod;
            ExecutionTime = executionTime;
            Exception = testMethod.Exception;
        }

        public Exception Exception { get; set; }
        public TimeSpan ExecutionTime { get; }
        public object Instance { get; }
        public TestMethod TestMethod { get; }

        public override string ToString()
        {
            if (Exception != null)
                return
                    $"{Instance.GetType().Name}.{TestMethod.Name} [FAILED], ExecutionTime {ExecutionTime}, Reason: {Exception.Message}";

            return
                $"{Instance.GetType().Name}.{TestMethod.Name} [SUCCESS], ExecutionTime {ExecutionTime}";
        }
    }
}