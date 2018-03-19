using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer;
using Coderr.Server.SqlServer.Tools;
using Griffin.ApplicationServices;
using Griffin.Container;
using Griffin.Data;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Jobs
{
    /// <summary>
    ///     Migrates data to our new collection table.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class RemoveOldCollectionDataJob : IBackgroundJobAsync
    {
        private readonly Importer _importer;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public RemoveOldCollectionDataJob(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            var db = (UnitOfWorkWithTransaction) unitOfWork;
            _importer = new Importer(db.Transaction);
        }

        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 2000)
            {
                var reportIds = new List<int>();
                using (var cmd = _unitOfWork.CreateDbCommand())
                {
                    cmd.CommandText = "SELECT TOP(10) Id, ContextInfo FROM ErrorReports WHERE ContextInfo != ''";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reportId = reader.GetInt32(0);
                            var json = reader.GetString(1);
                            var contexts = EntitySerializer.Deserialize<ErrorReportContextCollection[]>(json);
                            _importer.AddContextCollections(reportId, contexts);
                            reportIds.Add(reportId);
                        }
                    }
                }

                if (!reportIds.Any())
                    break;

                await _importer.Execute();
                _importer.Clear();

                using (var cmd = _unitOfWork.CreateDbCommand())
                {
                    var idStr = string.Join(",", reportIds);
                    cmd.CommandText = $"UPDATE ErrorReports SET ContextInfo='' WHERE Id IN({idStr})";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reportId = reader.GetInt32(0);
                            var json = reader.GetString(1);
                            var contexts = EntitySerializer.Deserialize<ErrorReportContextCollection[]>(json);
                            _importer.AddContextCollections(reportId, contexts);
                        }
                    }
                }
            }

            _unitOfWork.SaveChanges();
        }
    }
}