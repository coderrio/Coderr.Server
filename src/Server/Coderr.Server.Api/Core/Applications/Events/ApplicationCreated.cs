using System;
using DotNetCqs;
// ReSharper disable All

namespace codeRR.Server.Api.Core.Applications.Events
{
    /// <summary>
    ///     Published when a new application have been created by a user.
    /// </summary>
    [Message]
    public class ApplicationCreated
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ApplicationCreated" />.
        /// </summary>
        /// <param name="id">application identity</param>
        /// <param name="name">name as specified by the user</param>
        /// <param name="createdById">account id for the user that created the application</param>
        /// <param name="appKey">appKey used to identify the application during uploads.</param>
        /// <param name="sharedSecret">Used with <paramref name="appKey" /> to authenticate the upload.</param>
        public ApplicationCreated(int id, string name, int createdById, string appKey, string sharedSecret)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (appKey == null) throw new ArgumentNullException("appKey");
            if (sharedSecret == null) throw new ArgumentNullException("sharedSecret");
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            if (createdById <= 0) throw new ArgumentOutOfRangeException("createdById");

            CreatedById = createdById;
            AppKey = appKey;
            SharedSecret = sharedSecret;
            ApplicationId = id;
            ApplicationName = name;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected ApplicationCreated()
        {
        }

        /// <summary>
        ///     Application key which is used to identify the application that uploads a report.
        /// </summary>
        public string AppKey { get; set; }


        /// <summary>
        ///     Application identity
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     Name as entered by the user.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        ///     Account id of the person that created this application
        /// </summary>
        public int CreatedById { get; private set; }

        /// <summary>
        ///     Used together with the <see cref="AppKey" /> to be able to authenticate the upload.
        /// </summary>
        public string SharedSecret { get; set; }
    }
}