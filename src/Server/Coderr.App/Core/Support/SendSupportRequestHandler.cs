using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Support;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Core.Support
{
    /// <summary>
    ///     Sends a support request to the OneTrueError Team.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You must have bought commercial support or registered to get 30 days of free support.
    ///     </para>
    /// </remarks>
    [Component]
    public class SendSupportRequestHandler : ICommandHandler<SendSupportRequest>
    {
        /// <inheritdoc />
        public async Task ExecuteAsync(SendSupportRequest command)
        {
            var baseConfig = ConfigurationStore.Instance.Load<BaseConfiguration>();
            var errorConfig = ConfigurationStore.Instance.Load<OneTrueErrorConfigSection>();

            string email = null;
            var claim = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);
            if (claim != null)
                email = claim.Value;

            string installationId = null;
            if (email == null)
                email = baseConfig.SupportEmail;

            if (errorConfig != null)
            {
                if (errorConfig.ContactEmail != null)
                    email = errorConfig.ContactEmail;
                installationId = errorConfig.InstallationId;
            }

            var items = new List<KeyValuePair<string, string>>();
            if (installationId != null)
                items.Add(new KeyValuePair<string, string>("InstallationId", installationId));
            items.Add(new KeyValuePair<string, string>("ContactEmail", email));
            items.Add(new KeyValuePair<string, string>("Subject", command.Subject));
            items.Add(new KeyValuePair<string, string>("Message", command.Message));

            //To know which page the user had trouble with
            items.Add(new KeyValuePair<string, string>("PageUrl", command.Url));

            var content = new FormUrlEncodedContent(items);
            var client = new HttpClient();
            await client.PostAsync("https://onetrueerror.com/support/request", content);
        }
    }
}