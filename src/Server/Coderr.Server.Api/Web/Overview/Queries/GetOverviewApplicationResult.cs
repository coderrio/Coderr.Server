using System;
using System.Collections.Generic;

namespace codeRR.Server.Api.Web.Overview.Queries
{
    /// <summary>
    ///     Application specific result for <see cref="GetOverview" />
    /// </summary>
    public class GetOverviewApplicationResult
    {
        private readonly Dictionary<DateTime, int> _index = new Dictionary<DateTime, int>();

        /// <summary>
        ///     Creates a new instance of <see cref="GetOverviewApplicationResult" />.
        /// </summary>
        /// <param name="label">Application name</param>
        /// <param name="startDate">first day in sequence</param>
        /// <param name="days">Number of days that this result contains</param>
        /// <exception cref="ArgumentNullException">label</exception>
        /// <exception cref="ArgumentOutOfRangeException">days</exception>
        public GetOverviewApplicationResult(string label, DateTime startDate, int days)
        {
            if (label == null) throw new ArgumentNullException("label");
            if (days < 1) throw new ArgumentOutOfRangeException("days");

            Label = label;
            if (days == 1)
            {
                var startTime = DateTime.Today.AddHours(DateTime.Now.Hour).AddHours(-22);
                for (var i = 0; i < 24; i++)
                {
                    _index[startTime.AddHours(i)] = i;
                }
                Values = new int[24];
            }
            else
            {
                for (var i = 0; i < days; i++)
                {
                    _index[startDate.AddDays(i)] = i;
                }
                Values = new int[days];
            }
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetOverviewApplicationResult()
        {
        }

        /// <summary>
        ///     Label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     Values, one per day
        /// </summary>
        public int[] Values { get; set; }

        /// <summary>
        ///     Add another value
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentOutOfRangeException">value &lt; 0</exception>
        public void AddValue(DateTime date, int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException("value");

            //future date = malconfigured reporting clients.
            if (!_index.ContainsKey(date))
                return;
            
            var indexPos = _index[date];
            Values[indexPos] = value;
        }
    }
}