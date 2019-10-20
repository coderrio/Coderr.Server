using System;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContainerServiceAttribute : Attribute
    {
        public ContainerServiceAttribute(Lifetime lifetime = Lifetime.Scoped)
        {
            Lifetime = lifetime;
        }

        public Lifetime Lifetime { get; }
    }

    public enum Lifetime
    {
        Transient,
        Scoped,
        SingleInstance
    }
}