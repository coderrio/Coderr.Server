using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace codeRR.Server.ReportAnalyzer
{
    /// <summary>
    ///     Allows us to serialize properties with private setters.
    /// </summary>
    public class IncludeNonPublicMembersContractResolver : DefaultContractResolver
    {
        /// <summary>
        ///     Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given
        ///     <see cref="T:System.Reflection.MemberInfo" />.
        /// </summary>
        /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
        /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
        /// <returns>
        ///     A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given
        ///     <see cref="T:System.Reflection.MemberInfo" />.
        /// </returns>
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