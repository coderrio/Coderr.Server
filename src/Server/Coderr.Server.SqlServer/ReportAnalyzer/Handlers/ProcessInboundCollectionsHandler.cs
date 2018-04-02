using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.SqlServer.ReportAnalyzer.Jobs;
using Coderr.Server.SqlServer.Tools;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Handlers
{
    /// <summary>
    ///     Process collections that was attached to inbound reports
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Will convert them.
    ///     </para>
    /// </remarks>
    internal class ProcessInboundContextCollectionsHandler : IMessageHandler<ProcessInboundContextCollections>
    {
        private static int _isProcessing;
        private readonly Importer _importer;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ProcessInboundContextCollectionsHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            var db = (AnalysisUnitOfWork) unitOfWork;
            _importer = new Importer((SqlTransaction)db.Transaction);
        }


        public async Task HandleAsync(IMessageContext context, ProcessInboundContextCollections message)
        {
            // We can receive multiple reports simultaneously.
            // Make sure that we only got one handler running.
            if (Interlocked.CompareExchange(ref _isProcessing, 1, 0) == 1)
                return;

            try
            {
                var collections = await GetInboundCollections();
                foreach (var collection in collections)
                {
                    var contexts = EntitySerializer.Deserialize<ErrorReportContextCollection[]>(collection.JsonData);
                    _importer.AddContextCollections(collection.ReportId, contexts);
                }

                if (collections.Any())
                {
                    await _importer.Execute();
                    await DeleteImportedRows(collections);
                    _importer.Clear();
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
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