using System;

namespace Coderr.Server.App.Insights.Keyfigures.Application
{
    public class ItemWithDuration
    {
        public ItemWithDuration(int id, DateTime when, TimeSpan duration)
        {
            Id = id;
            When = when;
            Duration = duration;
        }

        public ItemWithDuration(int id, DateTime @when, DateTime? end)
        {
            Id = id;
            Duration = (end ?? DateTime.UtcNow).Subtract(when);
            When = when;
        }

        public DateTime When { get; set; }

        /// <summary>
        /// ApplicationId or AccountId
        /// </summary>
        public int Id { get; }
        public TimeSpan Duration { get; }
        
        public override string ToString()
        {
            return $"{Id} {Duration} ({When})";
        }
    }
}