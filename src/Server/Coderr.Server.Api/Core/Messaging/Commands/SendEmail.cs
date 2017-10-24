using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Messaging.Commands
{
    /// <summary>
    ///     Send an email.
    /// </summary>
    [Message]
    public class SendEmail
    {
        /// <summary>
        ///     Create a new instance of <see cref="SendEmail" />.
        /// </summary>
        /// <param name="message">Message to send</param>
        public SendEmail(EmailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            EmailMessage = message;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected SendEmail()
        {
        }

        /// <summary>
        ///     Message to send
        /// </summary>
        public EmailMessage EmailMessage { get; private set; }
    }
}