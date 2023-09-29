using System;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.Abstractions
{
    public class QueueConfig
    {
        public string ReportQueue { get; set; }
        public string InboundPartitions { get; set; }
        public string ReportEventQueue { get; set; }
        public string AppQueue { get; set; }

        public void Configure(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (ServerConfig.Instance.IsLive)
            {
                ReportQueue = config.GetSection("MessageQueue")["ReportQueue"];
                ReportEventQueue = config.GetSection("MessageQueue")["ReportEventQueue"];
                AppQueue = config.GetSection("MessageQueue")["AppQueue"];
                InboundPartitions = config.GetSection("MessageQueue")["InboundPartitions"];
            }
            else
            {
                ReportQueue = "ErrorReports";
                ReportEventQueue = "ErrorReportEvents";
                AppQueue = "Messaging";
                InboundPartitions = "InboundPartitions";
            }
        }
    }
}