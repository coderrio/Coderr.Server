using System;
using System.Collections.Generic;
using System.Security.Claims;
using Coderr.Server.Api.Insights.Queries;

namespace Coderr.Server.App.Metrics
{
    public class MetricGenerationContext : IMetricGenerationContext
    {
        private readonly IDictionary<int, int> _appIdFtes;
        internal readonly List<Metric> Metrics = new List<Metric>();
        internal readonly List<TrendLine> TrendLines = new List<TrendLine>();

        public MetricGenerationContext(MetricConfiguration configuration, ClaimsPrincipal user, IDictionary<int,int> appIdFtes)
        {
            _appIdFtes = appIdFtes;
            Configuration = configuration;
            User = user;
        }

        public void Add(Metric metric)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));
            Metrics.Add(metric);
        }

        public void Add(TrendLine trendLine)
        {
            if (trendLine == null) throw new ArgumentNullException(nameof(trendLine));
            TrendLines.Add(trendLine);
        }

        public MetricConfiguration Configuration { get; }
        public ClaimsPrincipal User { get;  }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int[] ApplicationIds { get; set; }
        public Dictionary<int, int> FullTimeEmployees { get; set; }
        public int GetNumberOfFtes(int applicationId)
        {
            if (_appIdFtes.TryGetValue(applicationId, out var ftes))
            {
                return ftes;
            }

            return 1;
        }

        public string GetApplicationName(int applicationId)
        {
            return "Ada";
        }
    }
    public interface IMetricGenerationContext
    {
        void Add(Metric metric);
        void Add(TrendLine metric);

        MetricConfiguration Configuration { get; }
        ClaimsPrincipal User { get; }


        /// <summary>
        /// Generated from the metric configuration
        /// </summary>
        DateTime From { get; }

        /// <summary>
        /// Generated from the metric configuration
        /// </summary>
        DateTime To { get; }


        /// <summary>
        /// Application ids to generate on.
        /// </summary>
        /// <remarks>
        ///<para>
        ///Have been collected using the configuration, requested application and the apps that the user have access to.
        /// </para>
        /// </remarks>
        int[] ApplicationIds { get; }

        ///<summary>
        /// Returns number of FTEs per applicationId.
        /// </summary>
        int GetNumberOfFtes(int applicationId);

        string GetApplicationName(int applicationId);
    }
}