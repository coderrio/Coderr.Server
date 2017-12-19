using System;
using System.Collections.Generic;
using System.Linq;

namespace codeRR.Server.Api.Core.Messaging
{
    /// <summary>
    ///     Used to send emails.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Used instead of the .NET classes to allow third party email services.
    ///     </para>
    /// </remarks>
    public class EmailMessage
    {
        /// <summary>
        ///     Create a new instance of <see cref="EmailMessage" />
        /// </summary>
        public EmailMessage()
        {
            Resources = new List<EmailResource>();
        }

        /// <summary>
        ///     Create a new instance of <see cref="EmailMessage" />
        /// </summary>
        /// <param name="recipient">Destination</param>
        public EmailMessage(string recipient)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            Recipients = new[] {new EmailAddress(recipient)};
            Resources = new List<EmailResource>();
        }

        /// <summary>
        ///     Create a new instance of <see cref="EmailMessage" />
        /// </summary>
        /// <param name="recipients">List of recipients</param>
        public EmailMessage(IReadOnlyList<string> recipients)
        {
            if (recipients == null) throw new ArgumentNullException("recipients");
            if (recipients.Count == 0) throw new ArgumentException("Tried to send to an empty list.", "recipients");

            Recipients = recipients.Select(x => new EmailAddress(x)).ToArray();
            Resources = new List<EmailResource>();
        }

        /// <summary>
        ///     Body (should be send as HTML)
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        ///     List of recipients
        /// </summary>
        public EmailAddress[] Recipients { get; set; }

        /// <summary>
        /// Whom should replies be sent to.
        /// </summary>
        public EmailAddress ReplyTo { get; set; }

        /// <summary>
        ///     Attachments and/or inline images.
        /// </summary>
        public IList<EmailResource> Resources { get; set; }

        /// <summary>
        ///     Subject line
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     Text body
        /// </summary>
        public string TextBody { get; set; }
    }
}