using System;

namespace TsGenerator
{
    public class GeneratedTypeScript
    {
        public string Code { get; set; }
        public Type DotNetType { get; set; }
        public string ModuleName { get; set; }
        public string TypeName { get; set; }
    }
}