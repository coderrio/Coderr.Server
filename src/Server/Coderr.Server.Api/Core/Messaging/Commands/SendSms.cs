using System;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Messaging.Commands
{
    /// <summary>
    ///     Send a cell phone text.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Requires a prepaid account at http://coderrapp.com/services/sms. Add your SMS Api key and the shared
    ///         secret in your web.config.
    ///     </para>
    ///     <example>
    ///         <![CDATA[
    /// <add key="Sms.ApiKey" value="SomeGuid" />
    /// <add key="Sms.SharedSecret" value="AnotherGuid" />
    /// ]]>
    ///     </example>
    /// </remarks>
    [Message]
    public class SendSms
    {
        /// <summary>
        ///     Creates a new instance of <see cref="SendSms" />.
        /// </summary>
        /// <param name="phoneNumber">
        ///     E.164 formatted number (<![CDATA[+<countryCode><areaCode><number>]]>, example: <c>+467012345</c>
        /// </param>
        /// <param name="message">Message. 160 chars is max for one SMS.</param>
        /// <exception cref="ArgumentNullException">phoneNumber; message</exception>
        public SendSms(string phoneNumber, string message)
        {
            if (phoneNumber == null) throw new ArgumentNullException("phoneNumber");
            if (message == null) throw new ArgumentNullException("message");

            PhoneNumber = phoneNumber;
            Message = message;
        }

        /// <summary>
        ///     Message. 160 chars is max for one SMS.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     E.164 formatted number (<![CDATA[+<countryCode><areaCode><number>]]>, example: <c>+467012345</c>
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}