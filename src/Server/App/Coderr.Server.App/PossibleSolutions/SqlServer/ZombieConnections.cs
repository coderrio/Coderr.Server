using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;

namespace Coderr.Server.App.PossibleSolutions.SqlServer
{
    [ContainerService]
    class ZombieConnections : ISolutionProvider
    {
        public Task SuggestSolutionAsync(SolutionProviderContext context)
        {
            if (!context.FullName.EndsWith("InvalidOperationException"))
                return Task.CompletedTask;

            if (!context.Description.Contains("This SqlTransaction has completed; it is no longer usable"))
                return Task.CompletedTask;

            if (!context.StackTrace.Contains("ZombieCheck"))
                return Task.CompletedTask;

            context.AddSuggestion(@"Try to run the transaction again.

```csharp
try
{
  // Invoke your code here
}
catch (InvalidOperationException ex)
{
    // not this error
    if (!ex.TargetSite.Contains(""ZombieCheck"")
       throw;
    

    // Invoke same code here.
}
```
", @"Server side error
================

SqlServer can rollback transactions if there are a server side error. That 
results in this error (when 'ZombieCheck' is present in the stack trace).

There is nothing that you can do to avoid this, except to try to run the transaction again.

[Read more](https://stackoverflow.com/questions/16692854/zombiecheck-exception-this-sqltransaction-has-completed-it-is-no-longer-usabl)
");
            return Task.CompletedTask;
        }
    }
}
