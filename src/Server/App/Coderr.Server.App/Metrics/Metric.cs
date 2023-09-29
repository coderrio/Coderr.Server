using System;

namespace Coderr.Server.App.Metrics
{
    public class Metric
    {
        public Metric(string name)
        {
            
        }
        /// <summary>
        /// Describes values (for instance of it's the worst or best values)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// This metric is for a specific application.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// This metric is for a specific account.
        /// </summary>
        /// <remarks>
        ///<para>
        ///This one can be combined with application id to tell that this metric is for that user in a specific application.
        /// </para>
        /// </remarks>
        public int AccountId { get; set; }

        public object Value { get; set; }

        public MetricChart Chart { get; set; }

        public object NormalizedValue { get; set; }
    }

    public class MetricChart
    {
        public DateTime Dates { get; set; }
        public ChartLine[] Lines { get; set; }
    }

    public class ChartLine
    {
        public int AccountId { get; set; }
        public int ApplicationId { get; set; }
        public int[] Values { get; set; }
    }
}