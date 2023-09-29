using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.ReportSpikes;
using Coderr.Server.ReportAnalyzer.ReportSpikes.Entities;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Spikes
{
    [ContainerService]
    internal class ReportSpikeRepository : IReportSpikeRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ReportSpikeRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<SpikeAggregation>> GetWeeksAggregations()
        {
            using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT ps.Id, a.Id ApplicationId, a.Name ApplicationName, TrackedDate, ReportCount, Notified,
                                            cast(PERCENTILE_CONT(0.50) WITHIN GROUP (ORDER BY ReportCount) OVER (PARTITION BY ApplicationId) as int) as Percentile50,
                                            cast(PERCENTILE_CONT(0.85) WITHIN GROUP (ORDER BY ReportCount) OVER (PARTITION BY ApplicationId) as int) as Percentile85
                                    FROM SpikeAggregation ps
                                    JOIN Applications a ON (a.Id = ApplicationId)
                                    WHERE TrackedDate >= @sevenDaysAgo
                                    ORDER BY ApplicationId, TrackedDate";
                cmd.AddParameter("sevenDaysAgo", DateTime.Today.AddDays(-7));
                return (IReadOnlyList<SpikeAggregation>)await cmd.ToListAsync<SpikeAggregation>();
                //using (var reader = await cmd.ExecuteReaderAsync())
                //{
                //    var aggregations = new List<SpikeAggregation>();
                //    while (await reader.ReadAsync())
                //    {
                //        var aggregation = new SpikeAggregation
                //        {
                //            Id = reader.GetInt32(0),
                //            ApplicationId = reader.GetInt32(1),
                //            ApplicationName = reader.GetString(2),
                //            TrackedDate = reader.GetDateTime(3),
                //            ReportCount = reader.GetInt32(4),
                //            Notified = reader.GetBoolean(5),
                //            Percentile50 = reader.GetInt32(6),
                //            Percentile85 = reader.GetInt32(7),
                //        };
                //        aggregations.Add(aggregation);
                //    }
                //}

                //return (int)numbers.Average();
            }
        }


        public async Task IncreaseReportCount(int applicationId, DateTime when)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"UPDATE SpikeAggregation
                                    SET ReportCount = ReportCount + 1 
                                    WHERE ApplicationId = @appId AND TrackedDate = @date
                                    IF @@ROWCOUNT = 0
                                      BEGIN
                                        INSERT SpikeAggregation (ApplicationId, TrackedDate, ReportCount, Notified)
                                        SELECT @appId, @date, 1, 0
                                      END";
                cmd.AddParameter("appId", applicationId);
                cmd.AddParameter("date", when);
                await cmd.ExecuteScalarAsync();
            }
        }

        public async Task MarkAsNotified(int spikeId)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"UPDATE SpikeAggregation SET Notified = 1 WHERE Id = @id";
                cmd.AddParameter("id", spikeId);
                await cmd.ExecuteScalarAsync();
            }
        }
    }
}