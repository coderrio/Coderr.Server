using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Accounts.Queries;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Api.Core.Support;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;


namespace Coderr.Server.App.Core.Support
{
    /// <summary>
    ///     Sends a support request to the Coderr Team.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You must have bought commercial support or registered to get 30 days of free support.
    ///     </para>
    /// </remarks>
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
            var errorConfig = _configStore.Load<CoderrConfigSection>();

            string email = null;
            var claim = context.Principal.FindFirst(ClaimTypes.Email);
            if (claim != null)
                email = claim.Value;
            else
            {
                var user = await context.QueryAsync(new GetAccountById(context.Principal.GetAccountId()));
                email = user.Email;
            }

            string installationId = null;
            if (string.IsNullOrEmpty(email))
                email = baseConfig.SupportEmail;

            if (errorConfig != null)
            {
                if (!string.IsNullOrEmpty(errorConfig.ContactEmail) && string.IsNullOrEmpty(errorConfig.ContactEmail))
                    email = errorConfig.ContactEmail;

                installationId = errorConfig.InstallationId;
            }

            // A support contact have been specified.
            // Thus this is a OnPremise/community server installation
            if (!string.IsNullOrEmpty(baseConfig.SupportEmail))
            {
                var msg = new EmailMessage(email)
                {
                    Subject = command.Subject,
                    ReplyTo = new EmailAddress(email),
                    TextBody = $"Request from: {email}\r\n\r\n{command.Message}"
                };
                var cmd = new SendEmail(msg);
                await context.SendAsync(cmd);
                return;
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
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
            await client.PostAsync("https://coderr.io/support/request", content);
        }
    }
}