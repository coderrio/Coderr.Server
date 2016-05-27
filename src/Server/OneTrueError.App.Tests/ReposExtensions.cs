using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;

namespace OneTrueError.App.Tests
{
    static class ReposExtensions
    {
        public static void AssignId<T>(this T instance, Action<T> action)
        {
            //repos.When(x => x.CreateAsync(Arg.Any<Account>())).Do(x => x.Arg<Account>().SetId(3));
            
        }
    }
}
