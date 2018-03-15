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

namespace codeRR.Server.SqlServer.Analysis.Jobs
{
    /// <summary>
    ///     Migrates data to our new collection table.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class RemoveOldCollectionDataJob : IBackgroundJobAsync
    {
        private readonly AnalysisDbContext _analysisDbContext;
        private readonly Importer _importer;

        public RemoveOldCollectionDataJob(AnalysisDbContext analysisDbContext)
        {
            _analysisDbContext = analysisDbContext;
            _importer = new Importer(analysisDbContext);
        }

        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 2000)
            {
                var reportIds = new List<int>();
                using (var cmd = _analysisDbContext.UnitOfWork.CreateDbCommand())
                {
                    cmd.CommandText = "SELECT TOP(10) Id, ContextInfo FROM ErrorReports WHERE cast([ContextInfo] as nvarchar(max)) != ''";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reportId = reader.GetInt32(0);
                            var json = reader.GetString(1);
                            var contexts = EntitySerializer.Deserialize<ErrorReportContext[]>(json);
                            _importer.AddContextCollections(reportId, contexts);
                            reportIds.Add(reportId);
                        }
                    }
                }

                if (!reportIds.Any())
                    break;

                await _importer.Execute();
                _importer.Clear();

                using (var cmd = _analysisDbContext.UnitOfWork.CreateDbCommand())
                {
                    var idStr = string.Join(",", reportIds);
                    cmd.CommandText = $"UPDATE ErrorReports SET ContextInfo='' WHERE Id IN({idStr})";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reportId = reader.GetInt32(0);
                            var json = reader.GetString(1);
                            var contexts = EntitySerializer.Deserialize<ErrorReportContext[]>(json);
                            _importer.AddContextCollections(reportId, contexts);
                        }
                    }
                }
            }

            _analysisDbContext.SaveChanges();
        }
    }
}