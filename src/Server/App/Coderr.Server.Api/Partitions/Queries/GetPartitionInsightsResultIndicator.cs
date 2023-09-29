using System;

namespace Coderr.Server.Api.Partitions.Queries
{
    public class GetPartitionInsightsResultIndicator
    {
        public GetPartitionInsightsResultIndicator(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public int Value { get; set; }
        public int PeriodValue { get; set; }

        public DateTime[] Dates { get; set; }
        public string[] Values { get; set; }
    }
}