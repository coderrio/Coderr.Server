using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.App.Modules.ReportSpikes;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Modules.ReportSpikes
{
    [Component]
    public class ReportSpikesRepository : IReportSpikeRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ReportSpikesRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public virtual async Task<int> GetAverageReportCountAsync(int applicationId)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"SELECT 
          [Day]  = DATENAME(WEEKDAY, createdatutc),
          Totals = cast (COUNT(*) as int)
        FROM errorreports
            WHERE applicationid=@appId
        GROUP BY 
          DATENAME(WEEKDAY, createdatutc)";
                cmd.AddParameter("appId", applicationId);
                var numbers = new List<int>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        numbers.Add((int) reader[1]);
                    }
                }
                numbers.Sort();

                RemovePeaks(numbers);
                return (int) numbers.Average();
            }
        }

        public async Task<ErrorReportSpike> GetSpikeAsync(int applicationId)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM ErrorReportSpikes WHERE ApplicationId = @appId AND SpikeDate = @date";
                cmd.AddParameter("date", DateTime.Today);
                cmd.AddParameter("appId", applicationId);
                return await cmd.FirstOrDefaultAsync<ErrorReportSpike>();
            }
        }

        public async Task<int> GetTodaysCountAsync(int applicationId)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    @"SELECT count(*) FROM ErrorReports WHERE ApplicationId = @appId AND CreatedAtUtc >= @date";
                cmd.AddParameter("date", DateTime.UtcNow.AddHours(-24));
                cmd.AddParameter("appId", applicationId);
                return (int) await cmd.ExecuteScalarAsync();
            }
        }

        public async Task CreateSpikeAsync(ErrorReportSpike spike)
        {
            await _unitOfWork.InsertAsync(spike);
        }

        public async Task UpdateSpikeAsync(ErrorReportSpike spike)
        {
            await _unitOfWork.UpdateAsync(spike);
        }

        private static void RemovePeaks(IList numbers)
        {
            if (numbers.Count > 3)
            {
                numbers.RemoveAt(0);
                numbers.RemoveAt(numbers.Count - 1);
            }
            if (numbers.Count > 3)
            {
                numbers.RemoveAt(0);
                numbers.RemoveAt(numbers.Count - 1);
            }
            if (numbers.Count > 3)
            {
                numbers.RemoveAt(0);
                numbers.RemoveAt(numbers.Count - 1);
            }
        }
    }
}