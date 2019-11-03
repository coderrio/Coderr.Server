using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coderr.Tests.Domain;

namespace Coderr.Tests
{
    public interface ITestDiscoverer
    {
        Task<TestClass> Find(Type testClassType);
        Task<IReadOnlyList<TestClass>> GetAll();
    }
}
