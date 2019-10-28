using System;
using System.Threading.Tasks;

namespace Coderr.IntegrationTests.Core.TestFramework
{
    internal static class ActionExtensions
    {
        public static async Task<bool> Retry(this Func<Task<bool>> x)
        {
            var delay = 200;
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                var result = await x();
                if (result) return true;

                await Task.Delay(delay);
                delay *= 2;
            }

            return false;
        }
    }
}