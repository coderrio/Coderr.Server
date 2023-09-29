using System.Collections.Generic;
using Coderr.Server.Abstractions.Config;

namespace Coderr.Server.ReportAnalyzer.UserNotifications
{
    /// <summary>
    ///     Configuration settings for browser notifications.
    /// </summary>
    public class BrowserNotificationConfig : IConfigurationSection//, IReportConfig
    {
        /// <summary>
        ///     Creates a new instance of <see cref="BrowserNotificationConfig" />
        /// </summary>
        public BrowserNotificationConfig()
        {
        }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        /// <summary>
        ///     Number of days to store reports.
        /// </summary>
        /// <remarks>
        ///     All reports older than this amount of days will be deleted.
        /// </remarks>
        public int RetentionDays { get; set; }

        string IConfigurationSection.SectionName => "BrowserNotificationConfig";

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
