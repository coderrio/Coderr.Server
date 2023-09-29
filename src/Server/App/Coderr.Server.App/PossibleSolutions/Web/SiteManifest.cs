using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;

namespace Coderr.Server.App.PossibleSolutions.Web
{
    [ContainerService]
    class SiteManifest : ISolutionProvider
    {
        /*TODO: 
         * 404 without this:
         		<staticContent>
			<mimeMap fileExtension=".json" mimeType="application/json" />
			<mimeMap fileExtension=".webmanifest" mimeType="application/manifest+json" />
		</staticContent>

         **/
        public Task SuggestSolutionAsync(SolutionProviderContext context)
        {
            if (!context.Description.EndsWith("site.webmanifest"))
            {
                return Task.CompletedTask;
            }

            context.AddSuggestion(@"Add a site.webmanifest to your web root.

Example

```javascript
{
    ""name"": ""HackerWeb"",
    ""short_name"": ""HackerWeb"",
    ""start_url"": ""."",
    ""display"": ""standalone"",
    ""background_color"": ""#fff"",
    ""description"": ""A simply readable Hacker News app."",
    ""icons"": [{
        ""src"": ""images/touch/homescreen48.png"",
        ""sizes"": ""48x48"",
        ""type"": ""image/png""
    }, {
        ""src"": ""images/touch/homescreen72.png"",
        ""sizes"": ""72x72"",
        ""type"": ""image/png""
    }, {
        ""src"": ""images/touch/homescreen96.png"",
        ""sizes"": ""96x96"",
        ""type"": ""image/png""
    }, {
        ""src"": ""images/touch/homescreen144.png"",
        ""sizes"": ""144x144"",
        ""type"": ""image/png""
    }, {
        ""src"": ""images/touch/homescreen168.png"",
        ""sizes"": ""168x168"",
        ""type"": ""image/png""
    }, {
        ""src"": ""images/touch/homescreen192.png"",
        ""sizes"": ""192x192"",
        ""type"": ""image/png""
    }],
    ""related_applications"": [{
        ""platform"": ""play"",
        ""url"": ""https://play.google.com/store/apps/details?id=cheeaun.hackerweb""
    }]
}
```

Then link it in your HTML header:

```html
<link rel=""manifest"" href=""/site.webmanifest"">
```", "site.webmanifset is used to the apperance of your application icon on the home screen of devices." +
      "" +
"More information at [MDN](https://developer.mozilla.org/en-US/docs/Web/Manifest)");

            return Task.CompletedTask;
        }
    }
}
