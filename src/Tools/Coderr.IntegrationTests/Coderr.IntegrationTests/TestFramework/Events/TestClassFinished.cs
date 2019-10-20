using System;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    public class TestClassFinished
    {
        public object TestClassInstance { get; }

        public TestClassFinished(object testClassInstance, TimeSpan swElapsed)
        {
            TestClassInstance = testClassInstance;
        }
    }
}