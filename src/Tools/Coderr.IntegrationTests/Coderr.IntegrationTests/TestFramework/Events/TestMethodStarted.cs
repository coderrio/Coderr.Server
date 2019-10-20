using System;
using System.Reflection;

namespace Coderr.IntegrationTests.Core.TestFramework.Events
{
    internal class TestMethodStarted
    {
        public TestMethodStarted(object instance, TestMethod method, DateTime startedAtUtc)
        {
            Instance = instance;
            Method = method;
            StartedAtUtc = startedAtUtc;
        }

        public object Instance { get; }
        public TestMethod Method { get; }
        public DateTime StartedAtUtc { get; }

        public override string ToString()
        {
            return $"Starting {Instance.GetType().Name}.{Method.Name} at {StartedAtUtc.ToShortTimeString()}.";
        }
    }
}