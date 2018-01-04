using System;
using Xunit.Sdk;

namespace codeRR.Server.Web.Tests.Helpers.xUnit
{
    public class EnsureTestEnvironmentAttribute : BeforeAfterTestAttribute
    {
        public static IisExpressHelper helper;

        public override void Before(System.Reflection.MethodInfo methodUnderTest)
        {
            if (!helper.IsRunning)
                throw new InvalidOperationException("IIS express failed to start.");
        }

        public override void After(System.Reflection.MethodInfo methodUnderTest)
        {
        }
    }
}