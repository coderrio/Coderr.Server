using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coderr.Tests.Domain;
using Coderr.Tests.Runners;

namespace Coderr.Tests.VisualStudio.TestAdapter
{
    internal class SharedData
    {
        public TestRunner TestRunner { get; set; }
        public ITestDiscoverer Discoverer { get; set; }
    }
    internal class TestData
    {
        public TestClass TestClass { get; set; }
        public SharedData Shared { get; set; }
        public TestMethod Method { get; set; }
    }
}
