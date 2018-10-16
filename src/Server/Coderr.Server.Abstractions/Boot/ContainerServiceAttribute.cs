using System;

namespace Coderr.Server.Abstractions.Boot
{
    public class ContainerServiceAttribute: Attribute
    {
        public bool IsSingleInstance { get; set; }
        public bool IsTransient { get; set; }
        public bool RegisterAsSelf { get; set; }
    }
}
