using System;

namespace Coderr.Server.App.Insights.Keyfigures.Application
{
    public class ItemWithCount
    {
        public ItemWithCount(int applicationId, DateTime when)
        {
            ApplicationId = applicationId;
            When = when;
        }

        public int AccountId { get; set; }

        public int ApplicationId { get; }

        public int Count { get; set; }

        public DateTime When { get; }

        public override string ToString()
        {
            return $"{ApplicationId} {When}";
        }
    }
}