using System;

namespace Coderr.Tests
{
    public class TestFailedException : Exception
    {
        public TestFailedException(Type testClass, string methodName, string errorMessage) : base(errorMessage)
        {
        }

        public TestFailedException(string errorMessage) : base(errorMessage)
        {
        }
    }
}