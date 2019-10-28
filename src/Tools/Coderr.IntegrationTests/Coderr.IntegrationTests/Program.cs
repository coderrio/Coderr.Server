using System;
using System.Reflection;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.TestFramework;
using Coderr.IntegrationTests.Core.TestFramework.Loggers;
using Coderr.IntegrationTests.Core.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.IntegrationTests.Core
{
    internal class Program
    {
        public const string DbName = "Coderr99";
        public const string ServerAddress = "http://localhost:50473";

        private static async Task Main(string[] args)
        {
            var client = new ApplicationClient(ServerAddress, DbName);
            var apiClient = await client.Open();

            var runner = new TestRunner();
            runner.RegisterServices(x =>
            {
                x.AddSingleton(client);
                x.AddSingleton<IEventReceiver>(new ConsoleLogger());
                x.AddSingleton(apiClient);
            });
            runner.Load(new[] {Assembly.GetExecutingAssembly()});

            await runner.RunAll();


            Console.ReadLine();
        }
    }
}