using System.Collections.Generic;
using System.Globalization;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.App.Modules.Messaging.Commands
{
    /// <summary>
    ///     Used to configure the <c>SmtpClient</c> which is part of .NET
    /// </summary>
    public sealed class DotNetSmtpSettings : IConfigurationSection
    {
        /// <summary>
        ///     Account name used to authenticate against the mail server
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        ///     Password for <see cref="AccountName" />.
        /// </summary>
        public string AccountPassword { get; set; }

        /// <summary>
        ///     Port number (25 or 587 depending on if SSL is used)
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        ///     Ip address or host name for the SMTP server
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        ///     Use SSL when communicating with the SMTP server
        /// </summary>
        public bool UseSsl { get; set; }

        string IConfigurationSection.SectionName
        {
            get { return "SmtpSettings"; }
        }

        IDictionary<string, string> IConfigurationSection.ToDictionary()
        {
            return new Dictionary<string, string>
            {
                {"AccountName", AccountName},
                {"AccountPassword", AccountPassword},
                {"SmtpHost", SmtpHost},
                {"PortNumber", PortNumber.ToString()},
                {"UseSSL", UseSsl.ToString(CultureInfo.InvariantCulture)}
            };
        }

        void IConfigurationSection.Load(IDictionary<string, string> items)
        {
            AccountName = items.GetString("AccountName");
            AccountPassword = items.GetString("AccountPassword", "");
            SmtpHost = items.GetString("SmtpHost");
            PortNumber = items.GetInteger("PortNumber", 0);
            UseSsl = items.GetBoolean("UseSSL", false);
        }
    }
}