using System.Collections.Generic;
using System.Linq;

namespace OneTrueError.Web.Areas.Receiver.ReportingApi
{
    public class NewReportContextInfo
    {
        public NewReportContextInfo()
        {
        }

        public NewReportContextInfo(string name, Dictionary<string, string> items)
        {
            Name = name;
            Items = items;
        }


        public string Name { get; set; }

        public Dictionary<string, string> Items { get; set; }

        public override string ToString()
        {
            return Name + " [" + string.Join(", ",
                Items.Select(x => x.Key + "=" + x.Value)) + "]";
        }
    }
}