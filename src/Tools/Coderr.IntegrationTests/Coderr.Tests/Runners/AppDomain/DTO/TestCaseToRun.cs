using System;

namespace Coderr.Tests.Runners.AppDomain.DTO
{
    [Serializable]
    public class TestCaseToRun
    {
        public TestCaseToRun(string fullyQualifiedName)
        {
            FullyQualifiedName = fullyQualifiedName;
        }

        public string TestClassName { get; set; }
        public string TestMethodName { get; set; }
        public string FullyQualifiedName { get; private set; }
    }
}