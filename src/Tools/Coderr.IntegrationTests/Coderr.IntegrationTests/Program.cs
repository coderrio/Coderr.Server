using System;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.TestFramework;
using Coderr.IntegrationTests.Core.TestFramework.Loggers;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.IntegrationTests.Core
{
    class Program
    {
        public const string DbName = "Coderr99";
        public const string ServerAddress = "http://localhost:50473";

        static async Task Main(string[] args)
        {
            var client = new ApplicationClient(ServerAddress, DbName);
            await client.Open();

            var runner = new TestRunner();
            runner.RegisterServices(x =>
            {
                x.AddSingleton(client);
                x.AddSingleton<IEventReceiver>(new ConsoleLogger());
            });
            runner.Load(new[] { Assembly.GetExecutingAssembly() });

            await runner.RunAll();


            Console.ReadLine();
        }
    }
}
