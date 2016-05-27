using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace OneTrueError.Web.Cqs
{
    [Serializable]
    public class Metadata
    {
        public Metadata(Type type)
        {
            FullTypeName = type.FullName;
            AssemblyName = type.Assembly.GetName().Name;
        }

        public Metadata()
        {
        }

        public string FullTypeName { get; set; }
        public string AssemblyName { get; set; }

        public byte[] Serialize()
        {
            var formatter = new BinaryFormatter
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeFormat = FormatterTypeStyle.TypesWhenNeeded
            };
            var ms = new MemoryStream();
            formatter.Serialize(ms, this);
            ms.Position = 0;
            var buffer = new byte[ms.Length];
            ms.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public static Metadata Parse(byte[] header)
        {
            var formatter = new BinaryFormatter
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeFormat = FormatterTypeStyle.TypesWhenNeeded
            };
            var ms = new MemoryStream(header);
            return (Metadata) formatter.Deserialize(ms);
        }

        public Type CreateType()
        {
            return Type.GetType(FullTypeName + ", " + AssemblyName);
        }
    }
}