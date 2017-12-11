using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Queries;
using codeRR.Server.Api.Core.Messaging.Commands;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Infrastructure.Net;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Messaging.Commands
{
    [Component]
    internal class SendEmailHandler : IMessageHandler<SendEmail>
    {
        private ConfigurationStore _configStore;

        public SendEmailHandler(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        public async Task HandleAsync(IMessageContext context, SendEmail command)
        {
            var client = CreateSmtpClient();
            if (client == null)
                return;

            var baseConfig = _configStore.Load<BaseConfiguration>();

            var email = new MailMessage
            {
                From = new MailAddress(baseConfig.SupportEmail),
                Subject = command.EmailMessage.Subject
            };
            foreach (var recipient in command.EmailMessage.Recipients)
            {
                int accountId;
                if (int.TryParse(recipient.Address, out accountId))
                {
                    var query = new GetAccountEmailById(accountId);
                    var emailAddress = await context.QueryAsync(query);
                    email.To.Add(new MailAddress(emailAddress, recipient.Name));
                }
                else
                    email.To.Add(new MailAddress(recipient.Address, recipient.Name));
            }
            if (string.IsNullOrEmpty(command.EmailMessage.HtmlBody))
            {
                email.Body = command.EmailMessage.TextBody;
                email.IsBodyHtml = false;
                await client.SendMailAsync(email);
                return;
            }

            var av = AlternateView.CreateAlternateViewFromString(command.EmailMessage.HtmlBody, null,
                MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(command.EmailMessage.TextBody))
                email.Body = command.EmailMessage.TextBody;
            foreach (var resource in command.EmailMessage.Resources)
            {
                var contentType = new ContentType(MimeMapping.GetMimeType(Path.GetExtension(resource.Name)));
                var linkedResource = new LinkedResource(resource.Name, contentType);
                await resource.Content.CopyToAsync(linkedResource.ContentStream);
                av.LinkedResources.Add(linkedResource);
            }
            email.AlternateViews.Add(av);
            await client.SendMailAsync(email);
        }

        private SmtpClient CreateSmtpClient()
        {
            var config = _configStore.Load<DotNetSmtpSettings>();
            if (string.IsNullOrEmpty(config.SmtpHost))
                return null;

            var client = new SmtpClient(config.SmtpHost, config.PortNumber);
            if (!string.IsNullOrEmpty(config.AccountName))
                client.Credentials = new NetworkCredential(config.AccountName, config.AccountPassword);
            client.EnableSsl = config.UseSsl;
            return client;
        }
    }
}