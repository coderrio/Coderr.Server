using System;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    public class TestFailedException : Exception
    {
        public TestFailedException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
