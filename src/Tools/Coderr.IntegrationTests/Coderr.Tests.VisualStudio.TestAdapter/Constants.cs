using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Coderr.Tests.VisualStudio.TestAdapter
{
    public static class Constants
    {
        public const string ExecutorUri = "executor://CoderrTests/v1";

        static Constants()
        {
            TestClassProperty = TestProperty.Register("TestClass", "Test class", typeof(string),
                TestPropertyAttributes.Hidden, typeof(Constants));
            TestMethodProperty = TestProperty.Register("TestMethod", "Test method", typeof(string),
                TestPropertyAttributes.Hidden, typeof(Constants));
        }

        public static TestProperty TestClassProperty { get; }

        public static TestProperty TestMethodProperty { get; }
    }
}