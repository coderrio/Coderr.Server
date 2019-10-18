using System;
using System.Threading.Tasks;

namespace Coderr.IntegrationTests.Core.Tools
{
    static class ActionExtensions
    {
        public static async Task<bool> Retry(this Func<Task<bool>> x)
        {
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                var result = await x();
                if (result)
                {
                    return true;
                }

                await Task.Delay(100);
            }

            return false;
        }
    }
}
