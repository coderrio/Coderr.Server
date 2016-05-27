using System;
using System.Configuration;

namespace OneTrueError.Infrastructure.Configuration.ConfigFile
{
    /// <summary>
    /// Key/value Configuration item 
    /// </summary>
    public class KeyValueElement : ConfigurationElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="KeyValueElement"/>.
        /// </summary>
        public KeyValueElement()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="KeyValueElement"/>.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <exception cref="ArgumentNullException">key; value</exception>
        public KeyValueElement(string key, string value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            Key = key;
            Value = value;
        }

        /// <summary>
        /// Key
        /// </summary>
        [ConfigurationProperty("key", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        /// <summary>
        /// Value
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}