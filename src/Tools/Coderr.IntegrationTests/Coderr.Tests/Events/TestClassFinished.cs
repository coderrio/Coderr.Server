using System;

namespace Coderr.Tests.Events
{
    public class TestClassFinished
    {
        public TestClassFinished(object testClassInstance, TimeSpan swElapsed)
        {
            TestClassInstance = testClassInstance;
        }

        public object TestClassInstance { get; }
    }
}