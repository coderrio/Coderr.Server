using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.SqlServer.Tools;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Analysis.Jobs
{
    /// <summary>
    ///     Takes inbound collections and do bulk insert on them
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class InsertCollectionsJob : IBackgroundJobAsync
    {
        private readonly AnalysisDbContext _dbContext;
        private readonly Importer _importer;

        public InsertCollectionsJob(AnalysisDbContext dbContext)
        {
            _dbContext = dbContext;
            _importer = new Importer(_dbContext);
        }

        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 2000)
            {
                var collections = await GetInboundCollections();
                foreach (var collection in collections)
                {
                    var contexts = EntitySerializer.Deserialize<ErrorReportContext[]>(collection.JsonData);
                    _importer.AddContextCollections(collection.ReportId, contexts);
                }

                if (!collections.Any())
                    break;

                await DeleteImportedRows(collections);
                await _importer.Execute();
                _importer.Clear();
            }

            _dbContext.SaveChanges();
        }


        private async Task DeleteImportedRows(IEnumerable<InboundCollection> collections)
        {
            using (var cmd = _dbContext.UnitOfWork.CreateDbCommand())
            {
                var ids = string.Join(",", collections.Select(x => x.Id).ToArray());
                var sql = $"DELETE FROM ErrorReportCollectionInbound WHERE Id IN ({ids})";
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
            }
        }


        private async Task<IList<InboundCollection>> GetInboundCollections()
        {
            using (var cmd = _dbContext.UnitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP(50) Id, ReportId, Body FROM ErrorReportCollectionInbound";
                return await cmd.ToListAsync<InboundCollection>();
            }
        }
    }
}