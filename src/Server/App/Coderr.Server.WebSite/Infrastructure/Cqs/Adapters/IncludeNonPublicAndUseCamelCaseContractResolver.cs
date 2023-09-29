using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Coderr.Server.WebSite.Infrastructure.Cqs.Adapters
{
    /// <summary>
    ///     Used by JSON.NET to be able to deserialize properties with private setters.
    /// </summary>
    public class IncludeNonPublicAndUseCamelCaseContractResolver : CamelCasePropertyNamesContractResolver
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
