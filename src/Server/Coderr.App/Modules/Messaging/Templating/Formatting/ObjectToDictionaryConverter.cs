using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.App.Modules.Messaging.Templating.Formatting
{
    /// <summary>
    ///     Converts an object into a dictionary (to be able to process it in the template)
    /// </summary>
    public class ObjectToDictionaryConverter
    {
        /// <summary>
        ///     Turn an object into a string which can be used for debugging.
        /// </summary>
        /// <param name="instance">Object to get a string representation for</param>
        /// <returns>"null" if the object is null, otherwise an string as given per object sample</returns>
        /// <remarks>
        ///     Look at the class doc for an example.
        /// </remarks>
        public Dictionary<string, object> Convert(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            var dictionary = new Dictionary<string, object>();
            ReflectObject(instance, "", dictionary);

            return dictionary;
        }

        /// <summary>
        ///     Checks if the specified type could be traversed or just added as a value.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns><c>true</c> if we should add this type as a value; <c>false</c> if we should do reflection on it.</returns>
        public static bool IsSimpleType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsPrimitive
                   || type == typeof(decimal)
                   || type == typeof(string)
                   || type == typeof(DateTime)
                   || type == typeof(int)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(TimeSpan);
        }

        /// <summary>
        ///     Use reflection on a complex object to add it's values to our context collection
        /// </summary>
        /// <param name="instance">Current object to reflect</param>
        /// <param name="prefix">Prefix, like "User.Address.Postal.ZipCode"</param>
        /// <param name="dictionary">Collection that values should be added to.</param>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength",
            Justification = "Null check is done on the argument.")]
        protected void ReflectObject(object instance, string prefix, IDictionary<string, object> dictionary)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (prefix == null) throw new ArgumentNullException("prefix");
            if (dictionary == null) throw new ArgumentNullException("dictionary");

            foreach (var propInfo in instance.GetType().GetProperties())
            {
                //TODO: Add support.
                if (propInfo.GetIndexParameters().Length != 0)
                    continue;

                var value = propInfo.GetValue(instance, null);
                if (value == null)
                {
                    dictionary.Add(prefix + propInfo.Name, "");
                    continue;
                }

                if (IsSimpleType(value.GetType()))
                {
                    dictionary.Add(prefix + propInfo.Name, value.ToString());
                }
                else
                {
                    var newPrefix = prefix == "" ? propInfo.Name + "." : prefix + propInfo.Name + ".";
                    ReflectObject(value, newPrefix, dictionary);
                }
            }
        }
    }
}