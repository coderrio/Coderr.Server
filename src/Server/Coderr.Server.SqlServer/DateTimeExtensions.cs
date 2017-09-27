using System;

namespace codeRR.Server.SqlServer
{
    public static class DateTimeExtensions
    {
        public static object ToDbNullable(this DateTime value)
        {
            return value == DateTime.MinValue ? (object) DBNull.Value : value;
        }
    }
}