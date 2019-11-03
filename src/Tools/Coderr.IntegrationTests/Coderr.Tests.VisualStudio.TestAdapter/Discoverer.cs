using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Coderr.Tests.VisualStudio.TestAdapter
{
    [DefaultExecutorUri(Constants.ExecutorUri)]
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    public class Discoverer : Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter.ITestDiscoverer
    {
        private static readonly HashSet<string> PlatformAssemblies =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "microsoft.visualstudio.testplatform.unittestframework.dll",
                "microsoft.visualstudio.testplatform.core.dll",
                "microsoft.visualstudio.testplatform.testexecutor.core.dll",
                "microsoft.visualstudio.testplatform.extensions.msappcontaineradapter.dll",
                "microsoft.visualstudio.testplatform.objectmodel.dll",
                "microsoft.visualstudio.testplatform.utilities.dll",
                "vstest.executionengine.appcontainer.exe",
                "vstest.executionengine.appcontainer.x86.exe",
                "coderr.tests.dll",
                "coderr.tests.visualstudio.testadapter.dll"
            };

        private readonly TestDiscoverer _discoverer = new TestDiscoverer();

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            var uri = new Uri(Constants.ExecutorUri);
            var foundTestClasses =
                _discoverer.LoadFromSources(
                    sources,
                    msg => logger.SendMessage(TestMessageLevel.Error, msg)
                );

            logger.SendMessage(TestMessageLevel.Informational, "Finding tests based on Coderr.Tests");
            foreach (var testClass in foundTestClasses)
            {
                foreach (var method in testClass.Methods)
                {
                    var name = $"{testClass.Type.FullName}.{method.Name}";
                    var testCase = new TestCase(name, uri, testClass.Source)
                    {
                        DisplayName = method.Name.Replace("_", " "),
                    };

                    testCase.SetPropertyValue(Constants.TestClassProperty, testClass.Type.FullName);
                    testCase.SetPropertyValue(Constants.TestMethodProperty, method.Name);
                    discoverySink.SendTestCase(testCase);
                }
            }
        }
    }
}