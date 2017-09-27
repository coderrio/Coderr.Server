using System;
using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Api.Core.Messaging
{
    /// <summary>
    ///     Email address
    /// </summary>
    public class EmailAddress
    {
        /// <summary>
        ///     Creates a new instance of <see cref="EmailAddress" />.
        /// </summary>
        /// <param name="address">email address or account id</param>
        public EmailAddress(string address)
        {
            if (address == null) throw new ArgumentNullException("address");

            var attr = new EmailAddressAttribute();
            int accountId;
            if (!attr.IsValid(address) && !int.TryParse(address, out accountId))
                throw new FormatException("'" + address + "' is not a valid email or account id.");

            Address = address;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected EmailAddress()
        {
        }

        /// <summary>
        ///     Email address or AccountId.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Recipient name
        /// </summary>
        public string Name { get; set; }
    }
}