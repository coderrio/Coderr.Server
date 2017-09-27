using System;

namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Key value pair
    /// </summary>
    public class KeyValuePair
    {
        /// <summary>
        ///     Creates a new instance of <see cref="KeyValuePair" />.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value (null is allowed)</param>
        /// <exception cref="ArgumentNullException">key; value</exception>
        public KeyValuePair(string key, string value)
        {
            if (key == null) throw new ArgumentNullException("key");

            Key = key;
            Value = value;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected KeyValuePair()
        {
        }

        /// <summary>
        ///     Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        ///     Value
        /// </summary>
        public string Value { get; private set; }
    }
}