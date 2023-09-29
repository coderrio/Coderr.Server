using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;
using Coderr.Server.App.Insights.Metrics;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights
{
    public abstract class CounterIndicator : IKeyPerformanceIndicatorGenerator
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        protected CounterIndicator(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public abstract Task CollectAsync(KeyPerformanceIndicatorContext context);

        protected async Task<List<ItemWithCount>> ExecuteCountSql(KeyPerformanceIndicatorContext context,
            string baseQuery, bool includeAccountId = false)
        {
            List<ItemWithCount> items;
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

                cmd.AddParameter("startDate", context.StartDate);
                cmd.AddParameter("endDate", context.EndDate);
                items = new List<ItemWithCount>();

                await FillCountItems(cmd, items, includeAccountId);
            }

            return items;
        }

        private static async Task FillCountItems(DbCommand cmd, ICollection<ItemWithCount> items, bool includeAccountId)
        {
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var item = new ItemWithCount(reader.GetInt32(0), reader.GetDateTime(1))
                    {
                        Count = reader.GetInt32(2)
                    };
                    if (includeAccountId) item.AccountId = reader.GetInt32(3);
                    items.Add(item);
                }
            }
        }
    }


    public abstract class CounterKeyMetricGenerator : IKeyMetricGenerator
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        protected CounterKeyMetricGenerator(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public abstract Task<KeyMetricDataResult> Collect(KeyMetricGeneratorContext context);

        protected async Task<ICollection<ItemWithCount>> ExecuteCountSql(KeyMetricGeneratorContext context,
            string baseQuery, bool includeAccountId = false)
        {
            List<ItemWithCount> items;
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = baseQuery;
                IEnumerable<int> appIds;
                if (context.ApplicationId == null)
                {
                    appIds = context.User.Claims
                        .Where(x => x.Type == CoderrClaims.Application)
                        .Select(x => int.Parse(x.Value));
                }
                else
                {
                    appIds = new[] {context.ApplicationId.Value};
                }

                var commaIds = string.Join(", ", appIds);
                cmd.CommandText = cmd.CommandText.Replace("#AppIdConstraint#",
                    $" AND ApplicationId IN ({commaIds}) ");
                cmd.AddParameter("startDate", DateTime.Today.AddDays(-context.DaysFrom));
                cmd.AddParameter("endDate", DateTime.Today.AddDays(1).AddSeconds(-1).AddDays(-context.DaysTo));
                items = new List<ItemWithCount>();

                await FillCountItems(cmd, items, includeAccountId);
            }

            return items;
        }

        private static async Task FillCountItems(DbCommand cmd, ICollection<ItemWithCount> items, bool includeAccountId)
        {
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var item = new ItemWithCount(reader.GetInt32(0), reader.GetDateTime(1))
                    {
                        Count = reader.GetInt32(2)
                    };
                    if (includeAccountId) item.AccountId = reader.GetInt32(3);
                    items.Add(item);
                }
            }
        }
    }
}