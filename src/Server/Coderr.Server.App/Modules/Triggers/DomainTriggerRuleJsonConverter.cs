using System;
using codeRR.Server.App.Modules.Triggers.Domain;
using codeRR.Server.App.Modules.Triggers.Domain.Rules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace codeRR.Server.App.Modules.Triggers
{
    /// <summary>
    ///     Handles our rule inheritance in a more elegant way
    /// </summary>
    public class DomainTriggerRuleJsonConverter : JsonCreationConverter<ITriggerRule>
    {
        /// <summary>
        ///     Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jsonObject">
        ///     contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected override ITriggerRule Create(Type objectType, JObject jsonObject)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            if (jsonObject == null) throw new ArgumentNullException("jsonObject");
            if (FieldExists("RuleType", jsonObject))
            {
                switch (jsonObject["RuleType"].ToString())
                {
                    case "Exception":
                        return new ExceptionRule();
                    case "ContextCollection":
                        return new ContextCollectionRule();
                }
            }

            if (jsonObject["ContextName"] != null)
            {
                return new ContextCollectionRule();
            }

            if (jsonObject["FieldName"] != null)
            {
                return new ExceptionRule();
            }

            throw new JsonReaderException("Unsupported rule: " + jsonObject);
        }

        private static bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}