using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Get information for an application either by using the key or application id
    /// </summary>
    [Message]
    public class GetApplicationInfo : Query<GetApplicationInfoResult>
    {
        private string _appKey;
        private int _applicationId;

        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationInfo" />.
        /// </summary>
        /// <param name="id">identity of the application</param>
        public GetApplicationInfo(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            ApplicationId = id;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationInfo" />.
        /// </summary>
        /// <param name="appKey">Application key used when sending error reports</param>
        public GetApplicationInfo(string appKey)
        {
            if (appKey == null) throw new ArgumentNullException("appKey");
            AppKey = appKey;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationInfo" />.
        /// </summary>
        protected GetApplicationInfo()
        {
        }

        /// <summary>
        ///     Application key from the user interface
        /// </summary>
        /// <exception cref="FormatException">Not a valid application key.</exception>
        public string AppKey
        {
            get { return _appKey; }
            set
            {
                Guid uid;
                if (!Guid.TryParse(value, out uid))
                    throw new FormatException("'" + value + "' is not a valid application key.");

                _appKey = value;
            }
        }

        /// <summary>
        ///     Application id
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Not a valid application id</exception>
        public int ApplicationId
        {
            get { return _applicationId; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid id.");
                _applicationId = value;
            }
        }
    }
}