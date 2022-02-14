using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coderr.Server.WebSite.Infrastructure
{
    public class KeyValuePairConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            var list = (List<KeyValuePair<string, string>>)value;
            writer.WriteStartArray();
            foreach (var item in list)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(item.Key);
                writer.WriteValue(item.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<KeyValuePair<string, string>>);
        }
    }
}
