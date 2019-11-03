using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Coderr.Tests.Runner.Resharper
{
    [UnitTestProvider, UsedImplicitly]
    public class ResharperTestProvider : IUnitTestProvider
    {
        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Unknown:
                    return !declaredElement.is;

                case UnitTestElementKind.Test:
                    return declaredElement.IsUnitTest();

                case UnitTestElementKind.TestContainer:
                    return declaredElement.IsUnitTestContainer();

                case UnitTestElementKind.TestStuff:
                    return declaredElement.IsAnyUnitTestElement();
            }

            return false;
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            throw new NotImplementedException();
        }

        public bool IsSupported(IHostProvider hostProvider, IProject project, TargetFrameworkId targetFrameworkId)
        {
            throw new NotImplementedException();
        }

        public bool IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            throw new NotImplementedException();
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            throw new NotImplementedException();
        }

        public bool SupportsResultEventsForParentOf(IUnitTestElement element)
        {
            throw new NotImplementedException();
        }

        public string ID { get; }
        public string Name { get; }
    }
}
