using System.Threading.Tasks;
using Coderr.Server.Abstractions.Incidents;

namespace Coderr.Server.App.PossibleSolutions.Web
{
    class LiveWriterSolutionProvider : ISolutionProvider
    {
        public Task SuggestSolutionAsync(SolutionProviderContext context)
        {
            if (!context.Description.Contains("wlwmanifest.xml"))
                return Task.CompletedTask;

            context.AddSuggestion(@"The file is a Windows Live Writer Manifest link

It enables Windows Live Writer to know how to connect to your website/blog.

Some WordPress bots looks for this file to detect word press sites.", "Can be ignored if you are not using Windows Live Writer.");
            return Task.CompletedTask;
        }
    }
}
