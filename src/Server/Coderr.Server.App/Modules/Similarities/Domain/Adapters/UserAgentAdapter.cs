using System;
using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner;
using UAParser;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters
{
    /// <summary>
    ///     Splits a user agent string into multiple context properties.
    /// </summary>
    public class UserAgentAdapter : IValueAdapter
    {
        /// <summary>
        ///     Adapt the value specified in the context
        /// </summary>
        /// <param name="context">Context information</param>
        /// <param name="currentValue">Value which might have been adapted</param>
        /// <returns>The new value (or same as the current value if no modification has been made)</returns>
        public object Adapt(ValueAdapterContext context, object currentValue)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (context.ContextName != "HttpHeaders" || context.PropertyName != "User-Agent" || context.Value == null)
                return currentValue;

            var uaParser = Parser.GetDefault();
            var c = uaParser.Parse(context.Value.ToString());
            context.AddCustomField("UserAgent.Family", c.UserAgent.Family);
            context.AddCustomField("UserAgent.Version",
                string.Format("{0} v{1}.{2}", c.UserAgent.Family, c.UserAgent.Major, c.UserAgent.Minor));


            context.AddCustomField("UserInfo", "DeviceType", c.Device.Family);
            context.AddCustomField("UserInfo", "IsWebSpider", c.Device.IsSpider);
            context.AddCustomField("UserInfo", "OS.Family", c.OS.Family);
            context.AddCustomField("UserInfo", "OS.Version", c.OS.Major + "." + c.OS.Minor);
            context.AddCustomField("UserInfo", "OS.VersionPatch", c.OS.Patch + "." + c.OS.PatchMinor);

            context.IgnoreProperty = true;
            return currentValue;
        }
    }
}