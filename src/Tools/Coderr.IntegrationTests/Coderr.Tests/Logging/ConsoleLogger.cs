using System;
using Coderr.Tests.Events;

namespace Coderr.Tests.Logging
{
    public class ConsoleLogger : IEventReceiver
    {
        public void Handle(object testEvent)
        {
            var t = testEvent as TestMethodFinished;
            if (t != null)
            {
                if (t.Result.Exception != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[FAILED] {t.Result.Exception.Message}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[OK]     ");
                    Console.WriteLine($"{t.Result.Elapsed.TotalMilliseconds:#}ms".PadLeft(8, ' '));
                }
            }
            else
            {
                Console.ResetColor();
                if (testEvent is TestMethodStarted t2)
                {
                    Console.Write(t2.Method.Name.PadRight(60, ' ').Replace('_', ' '));
                }
                else if (testEvent is TestClassStarted t3)
                {
                    Console.WriteLine(t3.TestClassInstance.GetType().Name);
                    Console.WriteLine("====================================");
                }
                else if (testEvent is TestClassFinished)
                {
                    Console.WriteLine();
                }
            }
        }
    }
}