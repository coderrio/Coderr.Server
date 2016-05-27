//using System;
//using System.IO;
//using System.Linq;
//using System.Net.Mail;
//using System.Threading.Tasks;
//using DotNetCqs;
//using Griffin.Container;
//using Griffin.Logging;
//using OneTrueError.Api.Core.Messaging;
//using OneTrueError.Api.Core.Messaging.Commands;
//using OneTrueError.App.Core.Accounts;
//using OneTrueError.App.Modules.Messaging.Templating;

//namespace OneTrueError.App.Modules.Messaging.Commands
//{
//    [Component]
//    public class SendEmailHandler : ICommandHandler<SendEmail>
//    {
//        private readonly ILogger _logger = LogManager.GetLogger<SendEmailHandler>();
//        private readonly IAccountRepository _accountRepository;

//        public SendEmailHandler(IAccountRepository accountRepository)
//        {
//            _accountRepository = accountRepository;
//        }


//        public async Task ExecuteAsync(SendEmail command)
//        {
//            foreach (var recipient in command.EmailMessage.Recipients)
//            {
//                int accountId;
//                if (int.TryParse(recipient.Address, out accountId))
//                {
//                    var user = await _accountRepository.GetByIdAsync(accountId);
//                    recipient.Address = user.Email;
//                }
//            }

//            var message = CreateForEmailMessage(command.EmailMessage);
//            await SendMessage(command, message);
//        }

//        private PostmarkMessage CreateForEmailMessage(EmailMessage message)
//        {
//            var from = new MailAddress("support@onetrueerror.com", "OneTrueError");
//            if (message.From != null && !string.IsNullOrEmpty(message.From.Address) &&
//                message.From.Address != "support@onetrueerror.com")
//                from = new MailAddress(message.From.Address, message.From.Name);

         
//            var msg = new PostmarkMessage
//            {
//                From = from.Address,
//                Subject = message.Subject,
//                HtmlBody = message.HtmlBody,
//                TextBody = message.TextBody
//            };

//            msg.To = string.Join(",", message.Recipients.Select(x => x.Address));
//            foreach (var resource in message.Resources)
//            {
//                var buf = new byte[resource.Content.Length];
//                resource.Content.Read(buf, 0, buf.Length);
//                resource.Content.Close();


//                var extension = Path.GetExtension(resource.Name);
//                var mimeType = MimeMapping.GetMimeType(extension);


//                msg.Attachments.Add(new PostmarkMessageAttachment
//                {
//                    Content = Convert.ToBase64String(buf),
//                    ContentId = resource.Name,
//                    ContentType = mimeType,
//                    Name = resource.Name
//                });
//            }

//            return msg;
//        }

//        private PostmarkMessage CreateForMailMessage(MailMessage message)
//        {
//            var from = new MailAddress("support@onetrueerror.com");
//            if (message.From != null && !string.IsNullOrEmpty(message.From.Address))
//                from = message.From;

//            return new PostmarkMessage
//            {
//                From = from.Address,
//                To = message.To[0].Address,
//                Subject = message.Subject,
//                HtmlBody = message.IsBodyHtml ? message.Body : "",
//                TextBody = message.IsBodyHtml ? "" : message.Body,
//            };
//        }

//        private async Task SendMessage(SendEmail command, PostmarkMessage message)
//        {
//            var client = new PostmarkClient("1ac67879-78aa-4c9f-ba69-e11d92458379");
//            var response =
//                await
//                    Task.Factory.FromAsync(client.BeginSendMessage(message), result => client.EndSendMessage(result),
//                        TaskCreationOptions.None);
//            if (response.Status != PostmarkStatus.Success)
//            {
//                if (command.EmailMessage != null)
//                    _logger.Error(string.Format("Failed to deliver email to {0}, Reason: {1}",
//                        string.Join(", ", command.EmailMessage.Recipients.Select(x => x.Address)), 
//                        response.Message));
//                //else
//                //    _logger.Error("Failed to deliver email to " + command.Message.To[0].Address + ", Reason: " + response.Message);
//            }
//        }
//    }
//}