using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Get an application by using the AppKey.
    /// </summary>
    [Message]
    public class GetApplicationIdByKey : Query<GetApplicationIdByKeyResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationIdByKey" />.
        /// </summary>
        /// <param name="applicationKey">appKey (GUID)</param>
        public GetApplicationIdByKey(string applicationKey)
        {
            if (applicationKey == null) throw new ArgumentNullException("applicationKey");
            Guid uid;
            if (!Guid.TryParse(applicationKey, out uid))
                throw new FormatException("'" + applicationKey + "' is not a valid application key.");
            ApplicationKey = applicationKey;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetApplicationIdByKey()
        {
        }

        /// <summary>
        ///     AppKey
        /// </summary>
        public string ApplicationKey { get; private set; }
    }
}