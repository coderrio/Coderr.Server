using System;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Boot
{
    public class ContainerServiceAttribute: Attribute
    {
        public bool IsSingleInstance { get; set; }
        public bool IsTransient { get; set; }
    }
}
