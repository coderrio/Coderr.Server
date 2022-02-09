using System;
using System.Collections.Generic;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Notifications.Commands
{
    public class SendBrowserNotification
    {
        public SendBrowserNotification(int accountIdToSendTo)
        {
            if (accountIdToSendTo <= 0) throw new ArgumentOutOfRangeException(nameof(accountIdToSendTo));
            AccountIdToSendTo = accountIdToSendTo;
        }

        public int AccountIdToSendTo { get; private set; }

        public IList<SendBrowserNotificationAction> Actions { get; set; } =
            new List<SendBrowserNotificationAction>();

        public string Badge { get; set; }

        public string Body { get; set; }


        public string IconUrl { get; set; }

        public string ImageUrl { get; set; }

        public string LanguageCode { get; set; } = "en";

        public bool RequireInteraction { get; set; }

        public string Tag { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string Title { get; set; } = "Push Demo";

        /// <summary>
        ///     Anonymous object
        /// </summary>
        public object UserData { get; set; }
    }
}