using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.TestFramework.Events;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    public class TestMethod
    {
        private readonly MethodInfo _methodInfo;

        public TestMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        public Exception Exception { get; set; }

        public TimeSpan ExecutionTime { get; set; }
        public DateTime StartedAtUtc { get; set; }

        public string Name => _methodInfo.Name;

        public async Task Invoke(object instance, IEventReceiver eventReceiver)
        {
            StartedAtUtc = DateTime.UtcNow;
            eventReceiver.Handle(new TestMethodStarted(instance, this, StartedAtUtc));
            var sw = Stopwatch.StartNew();
            try
            {
                var result = _methodInfo.Invoke(instance, null);
                if (result is Task t) await t;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            sw.Stop();
            ExecutionTime = sw.Elapsed;
            eventReceiver.Handle(new TestMethodFinished(instance, this, ExecutionTime));
        }
    }
}