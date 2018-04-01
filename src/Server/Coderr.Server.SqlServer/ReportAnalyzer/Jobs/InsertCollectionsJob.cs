using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer;
using Coderr.Server.SqlServer.Tools;
using Griffin.ApplicationServices;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Jobs
{
    /// <summary>
    ///     Takes inbound collections and do bulk insert on them
    /// </summary>
    [ContainerService]
    public class InsertCollectionsJob : IBackgroundJobAsync
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private readonly Importer _importer;

        public InsertCollectionsJob(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            var db = (UnitOfWorkWithTransaction)unitOfWork;
            _importer = new Importer(db.Transaction);
        }

        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 2000)
            {
                var collections = await GetInboundCollections();
                foreach (var collection in collections)
                {
                    var contexts = EntitySerializer.Deserialize<ErrorReportContextCollection[]>(collection.JsonData);
                    _importer.AddContextCollections(collection.ReportId, contexts);
                }

                if (!collections.Any())
                    break;

                await _importer.Execute();
                await DeleteImportedRows(collections);
                _importer.Clear();
            }

        }


        private async Task DeleteImportedRows(IEnumerable<InboundCollection> collections)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                var ids = string.Join(",", collections.Select(x => x.Id).ToArray());
                var sql = $"DELETE FROM ErrorReportCollectionInbound WHERE Id IN ({ids})";
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
            }
        }


        private async Task<IList<InboundCollection>> GetInboundCollections()
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP(50) Id, ReportId, Body FROM ErrorReportCollectionInbound";
                return await cmd.ToListAsync<InboundCollection>();
            }
        }
    }
}