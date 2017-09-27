using System;
using System.IO;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Core.Messaging;
using codeRR.Api.Core.Messaging.Commands;
using codeRR.App.Modules.Messaging.Templating;

namespace codeRR.App.Modules.Messaging.Commands
{
    /// <summary>
    ///     Send an email using a template.
    /// </summary>
    [Component]
    public class SendTemplateEmailHandler : ICommandHandler<SendTemplateEmail>
    {
        private readonly ICommandBus _commandBus;

        /// <summary>
        ///     Creates a new instance of <see cref="SendTemplateEmailHandler" />.
        /// </summary>
        /// <param name="commandBus">Used to send the <see cref="SendEmail" /> command.</param>
        /// <exception cref="ArgumentNullException">commandBus</exception>
        public SendTemplateEmailHandler(ICommandBus commandBus)
        {
            if (commandBus == null) throw new ArgumentNullException("commandBus");
            _commandBus = commandBus;
        }

        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task ExecuteAsync(SendTemplateEmail command)
        {
            var loader = new TemplateLoader();
            var templateParser = new TemplateParser();

            var layout = loader.Load("Layout");

            var template = loader.Load(command.TemplateName);
            var html = templateParser.RunAll(template, command.Model);
            if (html.IndexOf("src=\"cid:", StringComparison.OrdinalIgnoreCase) == -1)
                html = html.Replace(@"src=""", @"src=""cid:");

            string complete;
            try
            {
                complete = templateParser.RunFormatterOnly(layout,
                    new {Title = command.MailTitle, Body = html});
            }
            catch (Exception)
            {
                throw;
            }

            var msg = new EmailMessage(command.To) {Subject = command.Subject, HtmlBody = complete};

            foreach (var resource in template.Resources)
            {
                var linkedResource = new EmailResource(resource.Key, resource.Value);

                var reader = new BinaryReader(resource.Value);
                var dimensions = ImageHelper.GetDimensions(reader);
                var key = string.Format("src=\"cid:{0}\"", resource.Key);
                complete = complete.Replace(key,
                    string.Format("{0} width=\"{1}\" height=\"{2}\" style=\"border: 1px solid #000\"", key,
                        dimensions.Width, dimensions.Height));
                resource.Value.Position = 0;

                msg.Resources.Add(linkedResource);
            }
            foreach (var resource in layout.Resources)
            {
                var linkedResource = new EmailResource(resource.Key, resource.Value);

                var reader = new BinaryReader(resource.Value);
                var dimensions = ImageHelper.GetDimensions(reader);
                var key = string.Format("src=\"cid:{0}\"", resource.Key);
                complete = complete.Replace(key,
                    string.Format("{0} width=\"{1}\" height=\"{2}\"", key, dimensions.Width, dimensions.Height));
                resource.Value.Position = 0;

                msg.Resources.Add(linkedResource);
            }
            if (command.Resources != null)
            {
                foreach (var resource in command.Resources)
                {
                    msg.Resources.Add(resource);
                }
            }
            msg.HtmlBody = complete;

            var sendEmail = new SendEmail(msg);
            await _commandBus.ExecuteAsync(sendEmail);
        }
    }
}