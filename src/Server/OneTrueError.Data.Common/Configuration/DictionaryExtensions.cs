using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OneTrueError.Infrastructure.Configuration
{
    /// <summary>
    ///     Moves otherwise repeated conversions to a single place.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     Convert dictionary item to a boolean.
        /// </summary>
        /// <param name="dictionary">instance</param>
        /// <param name="name">Key</param>
        /// <returns>Value</returns>
        /// <exception cref="ArgumentException">If key is not present. Key name is included in the exception message.</exception>
        /// <exception cref="FormatException">Value is not a boolean. Includes key name and source value in the exception message.</exception>
        public static bool GetBoolean(this IDictionary<string, string> dictionary, string name)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (name == null) throw new ArgumentNullException("name");
            string value;
            if (!dictionary.TryGetValue(name, out value))
                throw new ArgumentException(string.Format("Failed to find key '{0}' in dictionary.", name));

            bool boolValue;
            if (!bool.TryParse(value, out boolValue))
                throw new FormatException(string.Format("Failed to convert '{0}' from value '{1}' to a boolean.", name,
                    value));
            return boolValue;
        }

        /// <summary>
        ///     Convert dictionary item to an integer.
        /// </summary>
        /// <param name="dictionary">instance</param>
        /// <param name="name">Key</param>
        /// <returns>Value</returns>
        /// <exception cref="ArgumentException">If key is not present. Key name is included in the exception message.</exception>
        /// <exception cref="FormatException">Value is not a boolean. Includes key name and source value in the exception message.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer")]
        public static int GetInteger(this IDictionary<string, string> dictionary, string name)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (name == null) throw new ArgumentNullException("name");
            string value;
            if (!dictionary.TryGetValue(name, out value))
                throw new ArgumentException(string.Format("Failed to find key '{0}' in dictionary.", name));

            int intValue;
            if (!int.TryParse(value, out intValue))
                throw new FormatException(string.Format("Failed to convert '{0}' from value '{1}' to an integer.",
                    name, value));
            return intValue;
        }

        /// <summary>
        ///     Get a string value.
        /// </summary>
        /// <param name="dictionary">instance</param>
        /// <param name="name">Key</param>
        /// <returns>Value</returns>
        /// <exception cref="ArgumentException">If key is not present. Key name is included in the exception message.</exception>
        public static string GetString(this IDictionary<string, string> dictionary, string name)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (name == null) throw new ArgumentNullException("name");
            string value;
            if (!dictionary.TryGetValue(name, out value))
                throw new ArgumentException(string.Format("Failed to find key '{0}' in dictionary.", name));

            return value;
        }

        /// <summary>
        ///     Get a string value.
        /// </summary>
        /// <param name="dictionary">instance</param>
        /// <param name="name">Key</param>
        /// <param name="defaultValue">Value to return if given key is not found.</param>
        /// <returns>Value if key is found; otherwise given default value.</returns>
        /// <exception cref="ArgumentException">If key is not present. Key name is included in the exception message.</exception>
        public static string GetString(this IDictionary<string, string> dictionary, string name, string defaultValue)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (name == null) throw new ArgumentNullException("name");
            string value;
            return !dictionary.TryGetValue(name, out value) ? defaultValue : value;
        }
    }
}