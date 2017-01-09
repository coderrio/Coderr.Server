using System;
using System.Collections.Generic;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Configuration
{
    /// <summary>
    ///     Base configuration for the OneTrueError service.
    /// </summary>
    public sealed class BaseConfiguration : IConfigurationSection
    {
        /// <summary>
        ///     Base URL for the home page, including protocol (http:// or https://)
        /// </summary>
        public Uri BaseUrl { get; set; }

        /// <summary>
        ///     Address used as "From" in all emails sent by the system.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        ///     Address to contact when having trouble with OneTrueError (account issues etc).
        /// </summary>
        public string SupportEmail { get; set; }

        string IConfigurationSection.SectionName
        {
            get { return "BaseConfig"; }
        }

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