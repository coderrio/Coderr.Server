using System;
using System.IO;

namespace codeRR.Server.Api.Core.Messaging
{
    /// <summary>
    ///     A resource attached to an email (typically an image)
    /// </summary>
    public class EmailResource
    {
        /// <summary>
        ///     Name of the resource (refered to using a <c>cid</c> in the email body)
        /// </summary>
        /// <param name="name">CID</param>
        /// <param name="content">Actual content</param>
        public EmailResource(string name, Stream content)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (content == null) throw new ArgumentNullException("content");
            Name = name;
            Content = content;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected EmailResource()
        {
        }

        /// <summary>
        ///     Contents of the resource. Stream must be readable.
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        ///     CID
        /// </summary>
        public string Name { get; set; }
    }
}