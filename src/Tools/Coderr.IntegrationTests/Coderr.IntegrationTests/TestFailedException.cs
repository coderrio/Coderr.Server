using System;

namespace Coderr.IntegrationTests.Core
{
    public class TestFailedException : Exception
    {
        public TestFailedException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
