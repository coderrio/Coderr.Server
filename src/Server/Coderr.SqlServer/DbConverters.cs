using System;
using codeRR.Server.SqlServer.Tools;

namespace codeRR.Server.SqlServer
{
    public class DbConverters
    {
        public static bool BoolFromByteArray(object arg)
        {
            return Convert.ToBoolean(((byte[]) arg)[0]);
        }

        public static TProperty DeserializeEntity<TProperty>(object arg)
        {
            return EntitySerializer.Deserialize<TProperty>((string) arg);
        }

        public static object SerializeEntity<T>(T arg)
        {
            return EntitySerializer.Serialize(arg);
        }


        public static DateTime ToEntityDate(object columnValue)
        {
            if (columnValue is DBNull)
                return DateTime.MinValue;

            return (DateTime) columnValue;
        }

        public static object ToNullableSqlDate(DateTime arg)
        {
            if (arg == DateTime.MinValue)
                return DBNull.Value;

            return arg;
        }

        public static object ToSqlNull(DateTime arg)
        {
            if (arg == DateTime.MinValue)
                return DBNull.Value;
            return arg;
        }
    }
}