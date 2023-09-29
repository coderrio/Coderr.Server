using System;

namespace Coderr.Server.App.Insights.Keyfigures.Application
{
    public class MonthDuration
    {
        public MonthDuration(DateTime @when, TimeSpan duration)
        {
            When = when;
            Duration = duration;
        }

        public int AccountId { get; set; }

        public int ApplicationId { get; set; }
        public TimeSpan Duration { get; set; }

        public bool HaveAccountId { get; set; }

        public DateTime When { get; set; }
    }
}