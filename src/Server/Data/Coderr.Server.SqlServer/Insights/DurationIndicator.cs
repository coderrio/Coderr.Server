using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights
{
    public abstract class DurationIndicator : IKeyPerformanceIndicatorGenerator
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        protected DurationIndicator(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public abstract Task CollectAsync(KeyPerformanceIndicatorContext context);

        /// <summary>
        ///     Execute count query
        /// </summary>
        /// <param name="context"></param>
        /// <param name="baseQuery"></param>
        /// <param name="includeAccountId">The query returns four columns where the last one is the account id. (Required for user indicators) </param>
        /// <returns></returns>
        protected async Task<List<MonthDuration>> ExecuteCountSql(KeyPerformanceIndicatorContext context,
            string baseQuery, bool includeAccountId = false)
        {
            List<MonthDuration> items;
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = baseQuery;
                if (context.ApplicationIds.Length == 0)
                {
                    cmd.CommandText = cmd.CommandText.Replace("#AppIdConstraint#", "");
                }
                else
                {
                    var commaIds = string.Join(", ", context.ApplicationIds);
                    cmd.CommandText = cmd.CommandText.Replace("#AppIdConstraint#",
                        $" AND ApplicationId IN ({commaIds}) ");
                }

                cmd.AddParameter("from", context.StartDate);
                cmd.AddParameter("to", context.EndDate);
                items = new List<MonthDuration>();

                await FillCountItems(cmd, items, includeAccountId);
            }

            return items;
        }

        private static async Task FillCountItems(DbCommand cmd, ICollection<MonthDuration> items, bool includeAccountId)
        {
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var value = decimal.ToDouble(reader.GetDecimal(2));
                    var item = new MonthDuration(reader.GetDateTime(1), TimeSpan.FromDays(value))
                    {
                        ApplicationId = reader.GetInt32(0)
                    };
                    if (includeAccountId)
                        item.AccountId = reader.GetInt32(3);

                    items.Add(item);
                }
            }
        }
    }
}