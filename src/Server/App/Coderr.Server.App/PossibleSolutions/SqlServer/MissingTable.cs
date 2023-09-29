using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;

namespace Coderr.Server.App.PossibleSolutions.SqlServer
{
    [ContainerService]
    internal class MissingTable : ISolutionProvider
    {
        public Task SuggestSolutionAsync(SolutionProviderContext context)
        {
            if (!context.FullName.EndsWith("SqlException"))
                return Task.CompletedTask;

            if (!context.Description.Contains("Invalid object name"))
                return Task.CompletedTask;

            context.AddSuggestion("Correct your database schema", "One of your database tables are missing. Add it ;)");
            return Task.CompletedTask;
        }
    }
}