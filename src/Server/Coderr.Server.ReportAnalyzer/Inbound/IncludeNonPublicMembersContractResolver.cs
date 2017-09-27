using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace codeRR.Server.ReportAnalyzer.Inbound
{
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