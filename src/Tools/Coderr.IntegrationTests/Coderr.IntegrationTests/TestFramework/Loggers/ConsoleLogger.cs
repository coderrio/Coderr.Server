using System;
using Coderr.IntegrationTests.Core.TestFramework.Events;

namespace Coderr.IntegrationTests.Core.TestFramework.Loggers
{
    internal class ConsoleLogger : IEventReceiver
    {
        public void Handle(object testEvent)
        {
            var t = testEvent as TestMethodFinished;
            if (t != null)
            {
                if (t.Exception != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[FAILED] {t.Exception.Message}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[OK]     ");
                    Console.WriteLine($"{t.ExecutionTime.TotalMilliseconds:#}ms".PadLeft(8, ' '));
                }
            }
            else
            {
                Console.ResetColor();
                if (testEvent is TestMethodStarted t2)
                {
                    Console.Write(t2.Method.Name.PadRight(60, ' '));
                }
                else if (testEvent is TestClassStarted t3)
                {
                    Console.WriteLine(t3.TestClassInstance.GetType().Name);
                    Console.WriteLine("====================================");
                }
            }
        }
    }
}