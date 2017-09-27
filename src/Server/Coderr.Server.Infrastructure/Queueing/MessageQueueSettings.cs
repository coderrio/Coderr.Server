using System.Collections.Generic;
using codeRR.Server.Infrastructure.Configuration;

namespace codeRR.Server.Infrastructure.Queueing
{
    public sealed class MessageQueueSettings : IConfigurationSection
    {
        public MessageQueueSettings()
        {
            UseSql = true;
        }

        public bool EventAuthentication { get; set; }

        public string EventQueue { get; set; }

        public bool EventTransactions { get; set; }

        public bool FeedbackAuthentication { get; set; }

        public string FeedbackQueue { get; set; }

        public bool FeedbackTransactions { get; set; }

        public bool ReportAuthentication { get; set; }

        public string ReportQueue { get; set; }

        public bool ReportTransactions { get; set; }

        public bool UseSql { get; set; }

        string IConfigurationSection.SectionName
        {
            get { return "MessageQueueing"; }
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