using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Coderr.Server.App.Modules.Messaging.Templating.Formatting
{
    public class JObjectReflector
    {
        private readonly Dictionary<string, object> _items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Checks if the specified type could be traversed or just added as a value.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns><c>true</c> if we should add this type as a value; <c>false</c> if we should do reflection on it.</returns>
        public static bool IsSimpleType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.IsPrimitive
                   || type == typeof(decimal)
                   || type == typeof(string)
                   || type == typeof(DateTime)
                   || type == typeof(int)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(TimeSpan);
        }

        public IDictionary<string, object> Reflect(JObject data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            _items.Clear();
            foreach (var prop in data) ReflectObject(prop.Value, prop.Key);

            return _items;
        }

        private void ReflectObject(JArray data, string prefix)
        {
            var index = 0;
            foreach (var item in data)
            {
                var childPrefix = $"{prefix}[{index++}]";
                ReflectObject(item, childPrefix);
            }
        }

        private void ReflectObject(JToken data, string prefix)
        {
            var isHandled = true;
            switch (data.Type)
            {
                case JTokenType.Array:
                    ReflectObject((JArray) data, prefix);
                    break;
                case JTokenType.Date:
                    _items.Add(prefix, data.ToObject<DateTime>());
                    break;

                case JTokenType.Boolean:
                    _items.Add(prefix, data.ToObject<bool>());
                    break;
                case JTokenType.Float:
                    _items.Add(prefix, data.ToObject<float>());
                    break;
                case JTokenType.Guid:
                    _items.Add(prefix, data.ToObject<Guid>());
                    break;
                case JTokenType.Integer:
                    _items.Add(prefix, data.ToObject<int>());
                    break;
                case JTokenType.String:
                    _items.Add(prefix, data.ToObject<string>());
                    break;
                case JTokenType.TimeSpan:
                    _items.Add(prefix, data.ToObject<TimeSpan>());
                    break;
                case JTokenType.Uri:
                    _items.Add(prefix, data.ToObject<string>());
                    break;
                case JTokenType.Null:
                    _items.Add(prefix, null);
                    break;
                default:
                    isHandled = false;
                    break;
            }

            if (isHandled)
                return;

            if (data is JObject)
                foreach (var prop in (JObject) data)
                    ReflectObject(prop.Value, $"{prefix}.{prop.Key}");
        }
    }
}