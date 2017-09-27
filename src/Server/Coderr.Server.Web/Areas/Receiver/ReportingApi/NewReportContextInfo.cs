using System;
using System.Collections.Generic;
using System.Linq;

namespace codeRR.Server.Web.Areas.Receiver.ReportingApi
{
    public class NewReportContextInfo
    {
        public NewReportContextInfo()
        {
        }

        public NewReportContextInfo(string name, Dictionary<string, string> properties)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            Name = name;
            Properties = properties;
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