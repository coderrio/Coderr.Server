using System;
using System.Collections.Generic;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.App.Configuration
{
    /// <summary>
    ///     Base configuration for the codeRR service.
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
        ///     Address to contact when having trouble with codeRR (account issues etc).
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