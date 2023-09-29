using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.IncidentInsights.Subscribers;
using Coderr.Server.Domain.Modules.Statistics;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.App.IncidentInsights
{
    [ContainerService]
    public class IncidentProgressRepository : IIncidentProgressRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        private readonly Dictionary<IncidentProgressCacheKey, List<IncidentProgressTracker>> _assignedCache =
            new Dictionary<IncidentProgressCacheKey, List<IncidentProgressTracker>>();

        private readonly Dictionary<IncidentProgressCacheKey, List<IncidentProgressTracker>> _closedCache =
            new Dictionary<IncidentProgressCacheKey, List<IncidentProgressTracker>>();

        private readonly Dictionary<IncidentProgressCacheKey, List<IncidentProgressTracker>> _createdCache =
            new Dictionary<IncidentProgressCacheKey, List<IncidentProgressTracker>>();

        public IncidentProgressRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<IncidentProgressTracker>> FindAssigned(int[] applicationIds, DateTime from, DateTime to)
        {
            var key = new IncidentProgressCacheKey(applicationIds, from, to);
            if (_assignedCache.TryGetValue(key, out var cacheItem))
                return cacheItem;

            var ids = string.Join(", ", applicationIds);
            var sql = $@"SELECT *
                        FROM CommonIncidentProgressTracking
                        WHERE ApplicationId IN ({ids})
                        AND AssignedAtUtc >= @from
                        AND AssignedAtUtc <= @to
                        ORDER BY CreatedAtUtc";

            var result = await _unitOfWork.ToListAsync<IncidentProgressTracker>(sql, new
            {
                from = from.Date,
                to
            });
            _assignedCache[key] = result;
            return result;
        }

        public async Task<List<IncidentProgressTracker>> FindCreated(int[] applicationIds, DateTime from, DateTime to)
        {
            var key = new IncidentProgressCacheKey(applicationIds, from, to);
            if (_createdCache.TryGetValue(key, out var cacheItem))
                return cacheItem;

            var ids = string.Join(", ", applicationIds);
            var sql = $@"SELECT *
                        FROM CommonIncidentProgressTracking
                        WHERE ApplicationId IN ({ids})
                        AND CreatedAtUtc >= @from
                        AND CreatedAtUTc <= @to
                        ORDER BY CreatedAtUtc";

            var result = await _unitOfWork.ToListAsync<IncidentProgressTracker>(sql, new {from, to});
            _createdCache[key] = result;
            return result;
        }

        public async Task<List<IncidentProgressTracker>> FindClosed(int[] applicationIds, DateTime from, DateTime to)
        {
            var key = new IncidentProgressCacheKey(applicationIds, from, to);
            if (_closedCache.TryGetValue(key, out var cacheItem))
                return cacheItem;

            var ids = string.Join(", ", applicationIds);
            var sql = $@"SELECT *
                        FROM CommonIncidentProgressTracking
                        WHERE ApplicationId IN ({ids})
                        AND ClosedAtUtc >= @from
                        AND ClosedAtUTc <= @to
                        ORDER BY CreatedAtUtc";

            var result = await _unitOfWork.ToListAsync<IncidentProgressTracker>(sql, new {from, to});
            _closedCache[key] = result;
            return result;
        }

        public IDictionary<int, int> CountAll(int[] applicationIds, DateTime from, DateTime to)
        {
            var ids = string.Join(", ", applicationIds);
            var sql = $@"SELECT ApplicationId, Count(*)
                        FROM CommonIncidentProgressTracking
                        WHERE ApplicationId IN ({ids})
                        AND (
                            (CreatedAtUtc >= @from AND CreatedAtUTc <= @to)
                             OR (AssignedAtUtc >= @from AND AssignedAtUtc <= @to)
                             OR (ClosedAtUtc >= @from AND ClosedAtUtc <= @to)
                        )
                        GROUP BY ApplicationId";
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("from", from);
                cmd.AddParameter("to", to);
                using (var reader = cmd.ExecuteReader())
                {
                    var items = new Dictionary<int, int>();
                    while (reader.Read()) items.Add(reader.GetInt32(0), reader.GetInt32(1));

                    return items;
                }
            }
        }
    }
}