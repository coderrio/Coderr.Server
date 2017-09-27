using System;
using Newtonsoft.Json;

namespace codeRR.Server.Infrastructure
{
    /// <summary>
    ///     Internal serializer, used only to store stuff that aren´t exposed outside the App/data namespace.
    /// </summary>
    public static class OneTrueSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        /// <summary>
        ///     Intern
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        /// <summary>
        ///     Deserialize JSON
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="type">type being deserialized</param>
        /// <returns>object</returns>
        public static object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, Settings);
        }

        /// <summary>
        ///     Serialize to JSON
        /// </summary>
        /// <param name="data">entity</param>
        /// <returns>JSON</returns>
        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}