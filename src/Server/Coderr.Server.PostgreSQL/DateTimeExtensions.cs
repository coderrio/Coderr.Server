using System;

namespace Coderr.Server.PostgreSQL
{
    public static class DateTimeExtensions
    {
        public static object ToDbNullable(this DateTime value)
        {
            return value == DateTime.MinValue ? (object) DBNull.Value : value;
        }
    }
}