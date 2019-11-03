using System;

namespace Coderr.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RunOnlyThisOneAttribute : Attribute
    {
    }
}