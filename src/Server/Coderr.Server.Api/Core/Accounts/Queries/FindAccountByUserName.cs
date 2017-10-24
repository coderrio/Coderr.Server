using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts.Queries
{
    /// <summary>
    ///     Find an account by the given user name
    /// </summary>
    [Message]
    public class FindAccountByUserName : Query<FindAccountByUserNameResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="FindAccountByUserName" />.
        /// </summary>
        /// <param name="userName">user name</param>
        public FindAccountByUserName(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            UserName = userName;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected FindAccountByUserName()
        {
        }

        /// <summary>
        ///     Username
        /// </summary>
        public string UserName { get; private set; }
    }
}