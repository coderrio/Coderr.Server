using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.Tests.Attributes;
using Coderr.Tests.Domain;

namespace Coderr.Tests
{
    /// <summary>
    ///     Scans assemblies for tests
    /// </summary>
    public class TestDiscoverer : ITestDiscoverer
    {
        private readonly List<TestClass> _testClasses = new List<TestClass>();

        public Task<TestClass> Find(Type testClassType)
        {
            return Task.FromResult(_testClasses.FirstOrDefault(x => x.Type == testClassType));
        }

        public Task<IReadOnlyList<TestClass>> GetAll()
        {
            return Task.FromResult((IReadOnlyList<TestClass>)_testClasses);
        }

        public TestClass[] Load(IEnumerable<Assembly> assemblies)
        {
            var types = (from assembly in assemblies
                         from type in assembly.GetTypes()
                         from method in type.GetMethods()
                         where method.GetCustomAttribute<TestAttribute>() != null
                         select type
                         ).Distinct();

            foreach (var type in types)
                _testClasses.Add(new TestClass(type));

            CheckForExplicitTests();

            return _testClasses.ToArray();
        }

        public TestClass[] Load(Assembly assembly, string source)
        {
            var types = (from type in assembly.GetTypes()
                         from method in type.GetMethods()
                         where method.GetCustomAttribute<TestAttribute>() != null
                         select type
                ).Distinct();

            foreach (var type in types)
                _testClasses.Add(new TestClass(type, source));

            CheckForExplicitTests();

            return _testClasses.ToArray();
        }

        private void CheckForExplicitTests()
        {
            var gotExclusive = _testClasses.Any(x => x.GotExclusiveFlag());
            if (!gotExclusive)
                return;

            foreach (var testClass in _testClasses)
            {
                testClass.MarkNonExclusive();
            }
        }


        public IReadOnlyList<TestClass> LoadFromSources(IEnumerable<string> sources, Action<string> logger)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var result = new List<TestClass>();
            foreach (var source in sources)
            {
                logger("Loading " + source);
                var assembly = LoadAssembly(source, logger);
                if (assembly == null)
                    continue;

                var found = Load(assembly, source);
                logger("Found " + string.Join(", ", found.Select(x => x.Type.Name)));
                if (found.Any())
                    result.AddRange(found);
            }

            return result;
        }

        private static Assembly LoadAssembly(string source, Action<string> errorLogger)
        {
            try
            {
                var path = Path.GetFullPath(source);
                return Assembly.LoadFile(path);
            }
            catch (Exception ex)
            {
                if (ex is ReflectionTypeLoadException e)
                {
                    foreach (var exception in e.LoaderExceptions)
                        errorLogger(
                            $"Failed to load assembly '{source}'. Tests cannot run. Reason: {exception.Message}");
                }
                else
                {
                    errorLogger(
                        $"Failed to load assembly '{source}'. Tests cannot run. Reason: {ex.Message}");
                }

                return null;
            }
        }

        public TestMethod Get(string testClassName, string testMethodName)
        {
            var tc = _testClasses.FirstOrDefault(x => x.Type.FullName == testClassName);
            return tc?.Methods.FirstOrDefault(x => x.Name == testMethodName);
        }

        public TestClass Find(string typeNameWithoutNamespace)
        {
            return _testClasses.FirstOrDefault(x => x.Type.Name == typeNameWithoutNamespace);
        }
    }
}