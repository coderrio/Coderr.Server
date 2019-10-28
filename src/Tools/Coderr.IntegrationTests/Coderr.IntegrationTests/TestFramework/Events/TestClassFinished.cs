using System;

namespace Coderr.IntegrationTests.Core.TestFramework.Events
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