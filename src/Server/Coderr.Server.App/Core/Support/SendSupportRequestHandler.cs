using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Support;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Support
{
    /// <summary>
    ///     Sends a support request to the codeRR Team.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You must have bought commercial support or registered to get 30 days of free support.
    ///     </para>
    /// </remarks>
    [Component]
    public class SendSupportRequestHandler : IMessageHandler<SendSupportRequest>
    {
        private ConfigurationStore _configStore;

        public SendSupportRequestHandler(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        /// <inheritdoc />
        public async Task HandleAsync(IMessageContext context, SendSupportRequest command)
        {
            var baseConfig = _configStore.Load<BaseConfiguration>();
            var errorConfig = _configStore.Load<codeRRConfigSection>();

            string email = null;
            var claim = context.Principal.FindFirst(ClaimTypes.Email);
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
            await client.PostAsync("https://coderrapp.com/support/request", content);
        }
    }
}