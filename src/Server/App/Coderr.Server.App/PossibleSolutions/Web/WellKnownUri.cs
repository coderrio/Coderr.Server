using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;

namespace Coderr.Server.App.PossibleSolutions.Web
{
    [ContainerService]
    public class WellKnownUri : ISolutionProvider
    {
        public Task SuggestSolutionAsync(SolutionProviderContext context)
        {
            if (!context.Description.Contains("/.wellknown/"))
            {
                return Task.CompletedTask;
            }

            var pos = context.Description.IndexOf("/.wellknown/") + "/.wellknown/".Length;
            var proto = context.Description.Substring(pos);
            context.AddSuggestion("Either 'Ignore' this incident so that it won't be reopened or add support for '" + proto + "'.",
                "Well-known services" +
                "======================" +
                "" +
                "The wellknown folder is used to provide metadata and policys for different web protocols." +
                "You can find a list at [wikipedia](https://en.wikipedia.org/wiki/List_of_/.well-known/_services_offered_by_webservers).");
            return Task.CompletedTask;
        }
    }
}
