using System;

namespace Coderr.Server.App.Insights.Keyfigures.Data
{
    /// <summary>
    /// Stores a calculated value of a KPI for a specific month
    /// </summary>
    public class KeyPerformanceIndicatorValue
    {
        public KeyPerformanceIndicatorValue(int applicationId, string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));

            ApplicationId = applicationId;
            Name = name;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            ValueType = value.GetType().FullName;
        }

        protected KeyPerformanceIndicatorValue()
        {

        }

        public int ApplicationId { get; set; }

        public int Id { get; private set; }

        /// <summary>
        /// Name of the key 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Value is stored as text, but can be of any type
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Stores value type (.NET type as FullName)
        /// </summary>
        public string ValueType { get; private set; }

        /// <summary>
        /// Name of value (if value is an account id, then this is the account name)
        /// </summary>
        public string ValueName { get; set; }

        /// <summary>
        /// Which period the KPI is for
        /// </summary>
        public DateTime YearMonth { get; set; }
    }
}
