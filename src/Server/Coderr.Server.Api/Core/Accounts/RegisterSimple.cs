using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Accounts
{
    /// <summary>
    ///     Register using email address only.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A temporary password is generated and included in the eamil. The user name is generated from
    ///         the name part of the email address.
    ///     </para>
    /// </remarks>
    public class RegisterSimple
    {
        /// <summary>
        ///     Create a new instance of <see cref="RegisterSimple" />.
        /// </summary>
        /// <param name="emailAddress">Email address</param>
        /// <exception cref="ArgumentNullException">emailAddress</exception>
        public RegisterSimple(string emailAddress)
        {
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");
            EmailAddress = emailAddress;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected RegisterSimple()
        {
        }

        /// <summary>
        ///     Email address
        /// </summary>
        public string EmailAddress { get; private set; }
    }
}