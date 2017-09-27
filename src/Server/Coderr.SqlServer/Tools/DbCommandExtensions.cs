using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace codeRR.Server.SqlServer.Tools
{
    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "Invoker have control over the CommandText.")]
    public static class DbCommandExtensions
    {
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static void Limit(this IDbCommand cmd, int count)
        {
            cmd.CommandText += string.Format(" OFFSET 0 ROWS FETCH NEXT {0} ROWS ONLY", count);
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static void Paging(this IDbCommand cmd, int pageNumber, int pageSize)
        {
            var offset = (pageNumber - 1)*pageSize;
            cmd.CommandText += string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, pageSize);
        }
    }
}