using System;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RunOnlyThisOneAttribute : Attribute
    {
    }
}