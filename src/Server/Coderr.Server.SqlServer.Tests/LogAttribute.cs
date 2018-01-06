using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace codeRR.Server.SqlServer.Tests
{
    public class LogAttribute : BeforeAfterTestAttribute
    {
        public static ITestOutputHelper Logger { get; set; }

        public override void Before(MethodInfo methodUnderTest)
        {
            Logger?.WriteLine("Running " + methodUnderTest.DeclaringType.Name + "." + methodUnderTest.Name);
            Debug.WriteLine("Running " + methodUnderTest.DeclaringType.Name + "." + methodUnderTest.Name);
            Console.WriteLine("Running " + methodUnderTest.DeclaringType.Name + "." + methodUnderTest.Name);
            base.Before(methodUnderTest);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Logger?.WriteLine("Completed " + methodUnderTest.DeclaringType.Name + "." + methodUnderTest.Name);
            Debug.WriteLine("Completed " + methodUnderTest.DeclaringType.Name + "." + methodUnderTest.Name);
            Console.WriteLine("Completed " + methodUnderTest.DeclaringType.Name + "." + methodUnderTest.Name);
            base.After(methodUnderTest);
        }
    }
}
