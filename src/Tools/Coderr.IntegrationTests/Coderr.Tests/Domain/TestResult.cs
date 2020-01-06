using System;

namespace Coderr.Tests.Domain
{
    public class TestResult
    {
        public TestResult(TestClass testClass, TestMethod method)
        {
            TestClass = testClass;
            Method = method;
            StartedAtUtc = DateTime.UtcNow;
        }

        public TimeSpan Elapsed
        {
            get
            {
                if (EndedAtUtc == DateTime.MinValue)
                    return TimeSpan.Zero;
                return EndedAtUtc.Subtract(StartedAtUtc);
            }
        }

        public DateTime EndedAtUtc { get; private set; }

        public Exception Exception { get; private set; }
        public bool IsIgnored { get; private set; }

        public bool IsSuccess { get; private set; }
        public TestMethod Method { get; }
        public DateTime StartedAtUtc { get; }
        public TestClass TestClass { get; }

        public void Failed(Exception exception)
        {
            EndedAtUtc = DateTime.UtcNow;
            Exception = exception;
        }

        public void Ignored()
        {
            IsIgnored = true;
            EndedAtUtc = DateTime.UtcNow;
        }


        public void Success()
        {
            IsSuccess = true;
            EndedAtUtc = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{(IsSuccess ? "  OK" : "FAIL")} {TestClass.Type.Name}.{Method.Name} {Exception?.Message}";
        }
    }
}