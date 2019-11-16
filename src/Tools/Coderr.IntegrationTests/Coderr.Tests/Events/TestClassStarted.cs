using System;

namespace Coderr.Tests.Events
{
    public class TestClassStarted
    {
        public TestClassStarted(object testClassInstance)
        {
            TestClassInstance = testClassInstance ?? throw new ArgumentNullException(nameof(testClassInstance));
            StartedAtUtc = DateTime.UtcNow;
        }

        public DateTime StartedAtUtc { get; }

        public object TestClassInstance { get; }
    }
}