using System;
using System.Reflection;
using Griffin.Cqs.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace codeRR.Web.Infrastructure.Cqs
{
    internal class CqsJsonNetSerializer : ICqsDeserializer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ContractResolver = new IncludeNonPublicMembersContractResolver(),
            TypeNameHandling = TypeNameHandling.None
        };

        public CqsJsonNetSerializer()
        {
            _settings.Converters.Add(new StringEnumConverter());
        }

        public object Deserialize(Type type, string message)
        {
            return JsonConvert.DeserializeObject(message, type, _settings);
        }

        public string Serialize(object message, out string contentType)
        {
            contentType = "application/json";
            return JsonConvert.SerializeObject(message, _settings);
        }

        public class IncludeNonPublicMembersContractResolver : DefaultContractResolver
        {
            //protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            //{
            //    var members = base.GetSerializableMembers(objectType);
            //    return members.Where(m => !m.Name.EndsWith("k__BackingField")).ToList();
            //}

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                //TODO: Maybe cache
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }
    }
}