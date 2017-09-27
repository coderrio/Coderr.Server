using System;

namespace codeRR.Server.App.Tests
{
    internal static class ReposExtensions
    {
        public static void AssignId<T>(this T instance, Action<T> action)
        {
            //repos.When(x => x.CreateAsync(Arg.Any<Account>())).Do(x => x.Arg<Account>().SetId(3));
        }
    }
}