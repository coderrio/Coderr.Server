using System;

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
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(t);
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(testEvent);
            }
        }
    }
}