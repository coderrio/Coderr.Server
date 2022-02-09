using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Coderr.Server.WebSite.Identity
{
    public static class DbTransactionExtensions
    {
        public static DbCommand CreateDbCommand(this IDbTransaction transaction)
        {
            var cmd = ((DbTransaction)transaction).Connection.CreateCommand();
            cmd.Transaction = (DbTransaction)transaction;
            return cmd;
        }
    }
}
