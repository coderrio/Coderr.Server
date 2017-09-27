using System;
using System.Collections.Generic;
using System.Linq;

namespace codeRR.Server.ReportAnalyzer.Inbound.Models
{
    public class NewReportContextInfo
    {
        public NewReportContextInfo()
        {
        }

        public NewReportContextInfo(string name, Dictionary<string, string> properties)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }


        public string Name { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public override string ToString()
        {
            return Name + " [" + string.Join(", ",
                Properties.Select(x => x.Key + "=" + x.Value)) + "]";
        }
    }
}