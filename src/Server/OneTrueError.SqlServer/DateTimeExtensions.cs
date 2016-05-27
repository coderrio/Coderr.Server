using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneTrueError.SqlServer
{
    public static class DateTimeExtensions
    {
        public static object ToDbNullable(this DateTime value)
        {
            return value == DateTime.MinValue ? (object) DBNull.Value : value;
        }

    }
}
