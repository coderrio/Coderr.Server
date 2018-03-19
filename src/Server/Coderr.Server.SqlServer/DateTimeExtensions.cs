using System;

namespace Coderr.Server.SqlServer
{
    public static class DateTimeExtensions
    {
        public static object ToDbNullable(this DateTime value)
        {
            return value == DateTime.MinValue ? (object) DBNull.Value : value;
        }
    }
}