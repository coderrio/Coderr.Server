using System;
using System.Collections.Generic;
using System.Linq;

namespace codeRR.Server.ReportAnalyzer.LibContracts
{
    /// <summary>
    ///     Context collection
    /// </summary>
    [Serializable]
    public class ProcessReportContextInfoDto
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ProcessReportContextInfoDto" />.
        /// </summary>
        protected ProcessReportContextInfoDto()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ProcessReportContextInfoDto" />.
        /// </summary>
        /// <param name="name">context collection name</param>
        /// <param name="properties">properties</param>
        /// <exception cref="ArgumentNullException">name; items</exception>
        public ProcessReportContextInfoDto(string name, Dictionary<string, string> properties)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (properties == null) throw new ArgumentNullException("properties");

            Name = name;
            Properties = properties;
        }


        /// <summary>
        ///     Context collection name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Properties
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Name + " [" + string.Join(", ",
                Properties.Select(x => x.Key + "=" + x.Value)) + "]";
        }
    }
}