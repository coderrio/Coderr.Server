using System;
using System.Data;
using System.Reflection;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.ApiKeys.Mappings
{
    public class MirrorMapper<T> : IEntityMapper<T> where T : new()
    {
        private MethodInfo[] _setters;

        public void Map(IDataRecord source, T destination)
        {
            Map(source, (object) destination);
        }

        public object Create(IDataRecord record)
        {
            return new T();
        }

        public void Map(IDataRecord source, object destination)
        {
            if (_setters == null)
                _setters = MapPropertySetters(source, typeof(T));

            for (var i = 0; i < _setters.Length; i++)
            {
                var value = source[i];
                if (value is DBNull)
                    continue;

                _setters[i].Invoke(destination, new[] {value});
            }
        }

        private MethodInfo[] MapPropertySetters(IDataRecord source, Type type)
        {
            var fields = new MethodInfo[source.FieldCount];
            for (var i = 0; i < source.FieldCount; i++)
            {
                var name = source.GetName(i);
                fields[i] =
                    type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .SetMethod;
            }
            return fields;
        }
    }
}