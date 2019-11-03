using Coderr.Tests.Domain;

namespace Coderr.Tests.Events
{
    public class TestMethodFinished
    {
        public TestMethodFinished(object instance, TestMethod testMethod, TestResult result)
        {
            Instance = instance;
            TestMethod = testMethod;
            Result = result;
        }

        public object Instance { get; }
        public TestResult Result { get; }
        public TestMethod TestMethod { get; }

        public override string ToString()
        {
            if (Result.Exception != null)
            {
                return
                    $"{Instance.GetType().Name}.{TestMethod.Name} [FAILED], ExecutionTime {Result.Elapsed}, Reason: {Result.Exception.Message}";
            }

            return
                $"{Instance.GetType().Name}.{TestMethod.Name} [SUCCESS], ExecutionTime {Result.Elapsed}";
        }
    }
}