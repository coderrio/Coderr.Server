using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.Infrastructure.Configuration
{
    /// <summary>
    ///     Moves otherwise repeated conversions to a single place.
    /// </summary>
    public static class ConfigDictionaryExtensions
    {
        /// <summary>
        ///     Convert dictionary item to a boolean.
        /// </summary>
        /// <param name="dictionary">instance</param>
        /// <param name="name">Key</param>
        /// <returns>Value</returns>
        /// <exception cref="ArgumentException">If key is not present. Key name is included in the exception message.</exception>
        /// <exception cref="FormatException">Value is not a boolean. Includes key name and source value in the exception message.</exception>
        public static bool GetBoolean(this IDictionary<string, string> dictionary, string name, bool? defaultValue = false)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (name == null) throw new ArgumentNullException("name");

            if (!dictionary.TryGetValue(name, out var value))
            {
                if (defaultValue != null)
                    return defaultValue.Value;
                throw new ArgumentException($"Failed to find key '{name}' in dictionary.");
            }

            if (defaultValue != null && value == null)
                return defaultValue.Value;

            if (!bool.TryParse(value, out var boolValue))
                throw new FormatException($"Failed to convert '{name}' from value '{value}' to a boolean.");

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
        public static int GetInteger(this IDictionary<string, string> dictionary, string name, int? defaultValue = 0)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (name == null) throw new ArgumentNullException("name");

            if (!dictionary.TryGetValue(name, out var value))
            {
                if (defaultValue != null)
                    return defaultValue.Value;
                throw new ArgumentException($"Failed to find key '{name}' in dictionary.");
            }

            if (value == null && defaultValue != null)
                return defaultValue.Value;

            if (!int.TryParse(value, out var intValue))
                throw new FormatException($"Failed to convert '{name}' from value '{value}' to an integer.");

            return intValue;
        }

        /// <summary>
        ///     Get a string value.
        /// </summary>
        /// <param name="dictionary">instance</param>
        /// <param name="name">Key</param>
        /// <returns>Value</returns>
        /// <exception cref="ArgumentException">If key is not present. Key name is included in the exception message.</exception>
        public static string GetString(this IDictionary<string, string> dictionary, string name, bool requireParameter = true)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (name == null) throw new ArgumentNullException("name");

            if (dictionary.TryGetValue(name, out var value))
                return value;

            if (requireParameter)
                throw new ArgumentException($"Failed to find key '{name}' in dictionary.");

            return null;
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
            return !dictionary.TryGetValue(name, out var value)
                ? defaultValue
                : value;
        }
    }
}