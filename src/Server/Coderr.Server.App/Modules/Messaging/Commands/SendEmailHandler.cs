using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Api.Core.Accounts.Queries;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.Infrastructure.Net;
using DotNetCqs;

using Markdig;

namespace Coderr.Server.App.Modules.Messaging.Commands
{
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

            // Emails have been disabled. Typically just in LIVE.
            if (string.IsNullOrEmpty(baseConfig.SupportEmail))
                return;

            var email = new MailMessage
            {
                From = new MailAddress(baseConfig.SupportEmail),
                Subject = command.EmailMessage.Subject
            };
            if (command.EmailMessage.ReplyTo != null)
            {
                var address = new MailAddress(command.EmailMessage.ReplyTo.Address,command.EmailMessage.ReplyTo.Name);
                email.ReplyToList.Add(address);
            }

            var markdownHtml = Markdown.ToHtml(command.EmailMessage.TextBody ?? "");

            foreach (var recipient in command.EmailMessage.Recipients)
            {
                if (int.TryParse(recipient.Address, out var accountId))
                {
                    var query = new GetAccountEmailById(accountId);
                    var emailAddress = await context.QueryAsync(query);
                    email.To.Add(new MailAddress(emailAddress, recipient.Name));
                }
                else
                    email.To.Add(new MailAddress(recipient.Address, recipient.Name));
            }
            if (string.IsNullOrEmpty(command.EmailMessage.HtmlBody) && markdownHtml == command.EmailMessage.TextBody)
            {
                email.Body = command.EmailMessage.TextBody;
                email.IsBodyHtml = false;
                await client.SendMailAsync(email);
                return;
            }

            if (string.IsNullOrEmpty(command.EmailMessage.HtmlBody))
                command.EmailMessage.HtmlBody = markdownHtml;

            var av = AlternateView.CreateAlternateViewFromString(command.EmailMessage.HtmlBody, null,
                MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(command.EmailMessage.TextBody))
                email.Body = command.EmailMessage.TextBody;
            foreach (var resource in command.EmailMessage.Resources)
            {
                var contentType = new ContentType(MimeMapping.GetMimeType(Path.GetExtension(resource.Name)));
                var ms = new MemoryStream(resource.Content, 0, resource.Content.Length, false);
                var linkedResource = new LinkedResource(ms)
                {
                    ContentId = resource.Name,
                    ContentType = contentType
                };
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