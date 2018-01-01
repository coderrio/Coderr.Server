using System;

namespace codeRR.Server.Web.Tests.Helpers.xUnit
{
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; set; }

        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
