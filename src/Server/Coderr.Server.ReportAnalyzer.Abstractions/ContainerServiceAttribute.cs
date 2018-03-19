using System;

namespace Coderr.Server.ReportAnalyzer.Abstractions
{
    public class ContainerServiceAttribute: Attribute
    {
        public bool IsSingleInstance { get; set; }
        public bool IsTransient { get; set; }
    }
}
