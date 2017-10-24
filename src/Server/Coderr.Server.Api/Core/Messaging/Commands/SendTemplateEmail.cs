using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Messaging.Commands
{
    /// <summary>
    ///     Send email using a template.
    /// </summary>
    [Message]
    public class SendTemplateEmail
    {
        /// <summary>
        ///     Creates a new instance of <see cref="SendTemplateEmail" />.
        /// </summary>
        /// <param name="mailTitle">Mail title (i.e. not the subject)</param>
        /// <param name="templateName">
        ///     Template to load (should be a sub folder to the invoking class, look at
        ///     <c>RequestPasswordResetHandler</c> for an example.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public SendTemplateEmail(string mailTitle, string templateName)
        {
            if (mailTitle == null) throw new ArgumentNullException("mailTitle");
            if (templateName == null) throw new ArgumentNullException("templateName");

            MailTitle = mailTitle;
            TemplateName = templateName;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected SendTemplateEmail()
        {
        }

        /// <summary>
        ///     Mail title (in the layout template)
        /// </summary>
        public string MailTitle { get; set; }

        /// <summary>
        ///     View model
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        ///     Resources to use in the template
        /// </summary>
        public EmailResource[] Resources { get; set; }

        /// <summary>
        ///     Mail subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     Name of the template to parse
        /// </summary>
        public string TemplateName { get; private set; }

        /// <summary>
        ///     Whom to send to (TODO: is accountId OK?)
        /// </summary>
        public string To { get; set; }
    }
}