using System;
using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;

namespace Coderr.Server.Infrastructure.Configuration
{
    /// <summary>
    ///     Base configuration for the Coderr service.
    /// </summary>
    public sealed class BaseConfiguration : IConfigurationSection
    {
        /// <summary>
        ///     allow new users to register accounts.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <c>null</c> = not configured = allow.
        ///     </para>
        /// </remarks>
        public bool? AllowRegistrations { get; set; }

        /// <summary>
        ///     Base URL for the home page, including protocol (http:// or https://)
        /// </summary>
        public Uri BaseUrl { get; set; }

        /// <summary>
        ///     Address used as "From" in all emails sent by the system.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        ///     Address to contact when having trouble with Coderr (account issues etc).
        /// </summary>
        public string SupportEmail { get; set; }

        string IConfigurationSection.SectionName => "BaseConfig";

        IDictionary<string, string> IConfigurationSection.ToDictionary()
        {
            return this.ToConfigDictionary();
        }

        void IConfigurationSection.Load(IDictionary<string, string> settings)
        {
            this.AssignProperties(settings);
        }
    }
}