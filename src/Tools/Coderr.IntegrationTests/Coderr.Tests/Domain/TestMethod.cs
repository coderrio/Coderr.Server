using System;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.Tests.Events;

namespace Coderr.Tests.Domain
{
    public class TestMethod
    {
        private readonly MethodInfo _methodInfo;

        public TestMethod(TestClass owner, MethodInfo methodInfo)
        {
            Owner = owner;
            _methodInfo = methodInfo;
        }


        public bool IsIgnored { get; set; }

        public string Name => _methodInfo.Name;
        public TestClass Owner { get; }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return _methodInfo.GetCustomAttribute<T>();
        }

        public async Task<TestResult> Invoke(object instance, IEventReceiver eventReceiver)
        {
            var result = new TestResult(Owner, this);

            if (IsIgnored)
            {
                result.Ignored();
                return result;
            }

            eventReceiver.Handle(new TestMethodStarted(instance, this, result.StartedAtUtc));
            try
            {
                var maybeTask = _methodInfo.Invoke(instance, null);
                if (maybeTask is Task t) await t;
                result.Success();
            }
            catch (Exception ex)
            {
                result.Failed(ex);
            }

            eventReceiver.Handle(new TestMethodFinished(instance, this, result));
            return result;
        }

        public override string ToString()
        {
            return $"{Name.Replace('_', ' ')}";
        }
    }
}