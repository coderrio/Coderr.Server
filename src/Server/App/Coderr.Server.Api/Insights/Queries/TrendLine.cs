using System;
using System.Collections.Generic;
using System.Linq;

namespace Coderr.Server.Api.Insights.Queries
{
    /// <summary>
    /// Contains a set of trend lines which represents a specific metric (like Error count).
    /// </summary>
    public class TrendLinePackage
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public IReadOnlyList<TrendLine> Lines { get; set; }
    }


    public class TrendLine
    {
        private readonly TrendLineValue[] _values;

        public TrendLine(string displayName, int valueCount, object defaultValue)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            _values = new TrendLineValue[valueCount];
            Fill(defaultValue);
        }

        public TrendLine(string displayName, TrendLineValue[] values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayNameId">0 = system</param>
        /// <param name="values">All values</param>
        public TrendLine(int displayNameId, TrendLineValue[] values)
        {
            if (displayNameId < 0) throw new ArgumentOutOfRangeException(nameof(displayNameId));
            _values = values ?? throw new ArgumentNullException(nameof(values));
            DisplayNameId = displayNameId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayNameId">0 = system</param>
        /// <param name="valueCount">Number of values to add</param>
        /// <param name="defaultValue">value to assign to each entry.</param>
        public TrendLine(int displayNameId, int valueCount, object defaultValue)
        {
            if (displayNameId < 0) throw new ArgumentOutOfRangeException(nameof(displayNameId));
            _values = new TrendLineValue[valueCount];
            Fill(defaultValue);
            DisplayNameId = displayNameId;
        }

        public bool IsCompanyAverage { get; set; }

        public string DisplayName { get; set; }

        public int DisplayNameId { get; set; }

        public IReadOnlyList<TrendLineValue> TrendValues => _values;

        public void Fill(object defaultValue)
        {
            for (var i = 0; i < _values.Length; i++)
                _values[i] = new TrendLineValue(defaultValue);
        }

        public void Assign(int monthIndex, object value, object normalizedValue = null)
        {
            _values[monthIndex].Value = value;
            if (normalizedValue != null)
            {
                _values[monthIndex].Normalized = normalizedValue;
            }
        }

        public override string ToString()
        {
            return DisplayName + " [" + string.Join(", ", _values.Select(x => x.Value)) + "]";
        }
    }
}