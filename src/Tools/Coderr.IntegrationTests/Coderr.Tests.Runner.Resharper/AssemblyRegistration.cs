using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
//https://github.com/JohnStov/ReSharperFixieRunner/tree/master/FixiePlugin
namespace Coderr.Tests.Runner.Resharper
{
    [SolutionComponent]
    public class AssemblyRegistration
    {
        public AssemblyRegistration(UnitTestingAssemblyLoader assemblyLoader)
        {
            // Register assemblies needed later by test runner.
            assemblyLoader.RegisterAssembly(typeof(TaskRunner).Assembly);
        }
    }
}