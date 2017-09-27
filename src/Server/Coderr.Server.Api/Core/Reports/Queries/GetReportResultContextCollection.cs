using System;

namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Context collection for <see cref="GetReportResultContextCollection" />.
    /// </summary>
    public class GetReportResultContextCollection
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetReportResultContextCollection" />.
        /// </summary>
        /// <param name="name">collection name</param>
        /// <param name="properties">all uploaded properties</param>
        /// <exception cref="ArgumentNullException">name; properties</exception>
        public GetReportResultContextCollection(string name, KeyValuePair[] properties)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (properties == null) throw new ArgumentNullException("properties");

            Name = name;
            Properties = properties;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected GetReportResultContextCollection()
        {
        }

        /// <summary>
        ///     Context collection name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Properties.
        /// </summary>
        public KeyValuePair[] Properties { get; set; }
    }
}